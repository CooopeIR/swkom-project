﻿using AutoMapper;
using DocumentDAL.Entities;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace SWKOM.Services
{
    /// <summary>
    /// RabbitMQ management functions (start connection, close connection, connect to RabbitMQ, start listening)
    /// </summary>
    public class RabbitMqListenerService : IHostedService
    {
        private readonly IConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ElasticsearchClient _elasticClient;

        /// <summary>
        /// Start connection and listening RabbitMQ
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectToRabbitMQ();
            StartListening();
            return Task.CompletedTask;
        }
        /// <summary>
        /// Constructor for RabbitMqListenerService class; initialization of variables
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="connectionFactory"></param>

        public RabbitMqListenerService(IHttpClientFactory httpClientFactory, IConnectionFactory connectionFactory, ElasticsearchClient client)
        {
            _httpClientFactory = httpClientFactory;
            _connectionFactory = connectionFactory;
            _elasticClient = client;
        }

        private void ConnectToRabbitMQ()
        {
            int retries = 15;
            while (retries > 0)
            {
                try
                {
                    _connection = _connectionFactory.CreateConnection();

                    _channel = _connection.CreateModel();
                    _channel.QueueDeclare(queue: "ocr_result_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine("Erfolgreich mit RabbitMQ verbunden und Queue erstellt.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Verbinden mit RabbitMQ: {ex.Message}. Versuche es in 5 Sekunden erneut...");
                    Thread.Sleep(5000);
                    retries--;
                }
            }
            if (_connection == null || !_connection.IsOpen)
            {
                throw new Exception("Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen.");
            }
        }

        private void StartListening()
        {
            try
            {
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var parts = message.Split('|', 2);
                    Console.WriteLine($@"[Listener] Nachricht erhalten: {message}");

                    if (parts.Length == 2)
                    {
                        var id = parts[0];
                        var extractedText = parts[1];
                        if (string.IsNullOrEmpty(extractedText))
                        {
                            Console.WriteLine($@"Fehler: Leerer OCR-Text für Task {id}. Nachricht wird ignoriert.");
                            return;
                        }
                        var client = _httpClientFactory.CreateClient("DocumentDAL");
                        var response = await client.GetAsync($"/api/document/{id}");
                        if (response.IsSuccessStatusCode)
                        {
                            var documentItem = await response.Content.ReadFromJsonAsync<DocumentItem>();
                            if (documentItem != null)
                            {
                                Console.WriteLine($@"[Listener] Document {id} erfolgreich abgerufen.");
                                Console.WriteLine($@"[Listener] OCR Text für Document {id}: {extractedText}");
                                Console.WriteLine($@"[Listener] Document vor Update: {documentItem.Id}, {documentItem.Title}, {documentItem.Author}");

                                documentItem.OcrText = extractedText;

                                //var payload = JsonSerializer.Serialize(documentItem);
                                ////Console.WriteLine($"Payload: {payload}");

                                var existsResponse = await _elasticClient.Indices.ExistsAsync("documents");
                                if (!existsResponse.Exists)
                                {
                                    var createResponse = await _elasticClient.Indices.CreateAsync("documents");
                                    if (!createResponse.Acknowledged)
                                    {
                                        Console.WriteLine("Failed to create the index.");
                                    }
                                }

                                var indexResponse =
                                    await _elasticClient.IndexAsync(documentItem, i => i.Index("documents"));

                                if (!indexResponse.IsValidResponse)
                                {
                                    Console.WriteLine($@"Fehler beim indexieren des Dokuments mit ID {id}");
                                }

                                

                                var updateResponse = await client.PutAsJsonAsync($"/api/document/{id}", documentItem);
                                if (!updateResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($@"Fehler beim Aktualisieren des Dokuments mit ID {id}");
                                    Console.WriteLine($"{updateResponse.StatusCode} - {updateResponse.Content}");
                                }
                                else
                                {
                                    Console.WriteLine($@"OCR Text für Dokument {id} erfolgreich aktualisiert.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($@"[Listener] Dokument mit Id {id} nicht gefunden.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($@"Fehler beim Abrufen des Dokuments mit ID {id}: {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Console.WriteLine(@"Fehler: Ungültige Nachricht empfangen.");
                    }
                };
                _channel.BasicConsume(queue: "ocr_result_queue", autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Fehler beim Starten des Listeners für OCR-Ergebnisse: {ex.Message}");
            }
        }
        /// <summary>
        /// Close channel and connection of RabbitMQ
        /// </summary>
        /// <param name="cancellationToken"></param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return Task.CompletedTask;
        }
    }
}