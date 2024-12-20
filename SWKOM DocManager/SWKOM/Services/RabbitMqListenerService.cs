﻿using AutoMapper;
using DocumentDAL.Entities;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SWKOM.Services
{
    /// <summary>
    /// RabbitMQ management functions (start connection, close connection, connect to RabbitMQ, start listening)
    /// </summary>
    public class RabbitMqListenerService : IHostedService
    {
        //private readonly IConnectionFactory _connectionFactory;
        private IConnection? _connection = null;
        private IModel? _channel = null;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMessageQueueService _messageQueueService;


        /// <summary>
        /// Start connection and listening RabbitMQ
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Initialize();
            await StartListening();
        }

        /// <summary>
        /// Constructor for RabbitMqListenerService class; initialization of variables
        /// </summary>
        /// <param name="httpClientFactory">IHttpClientFactory httpClientFactory</param>
        /// <param name="messageQueueService">IMessageQueueService messageQueueService</param>
        public RabbitMqListenerService(IHttpClientFactory httpClientFactory, IMessageQueueService messageQueueService)
        {
            _httpClientFactory = httpClientFactory;
            //_connectionFactory = connectionFactory;
            _messageQueueService = messageQueueService;
        }
        private Task Initialize()
        {
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    if (_connection == null)
                    {
                        _connection = _messageQueueService.Connection;
                        _channel = _connection?.CreateModel();
                    }

                    // Queue to consume OCR Results
                    _channel?.QueueDeclare(queue: "ocr_result_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    // Queue to publish DocumentItems for Indexing Worker
                    _channel?.QueueDeclare(queue: "indexing_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    Console.WriteLine("[Listener] Erfolgreich mit RabbitMQ verbunden und Queue erstellt.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Listener] Fehler beim Verbinden mit RabbitMQ: {ex.Message}. Versuche es in 5 Sekunden erneut...");
                    Thread.Sleep(10000);
                    retries--;
                }
            }
            if (_connection is not { IsOpen: true })
            {
                throw new Exception("Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen.");
            }
            return Task.CompletedTask;
        }

        private async Task StartListening()
        {
            try
            {
                if (_channel != null && _connection != null && (!_connection.IsOpen || !_channel.IsOpen))
                    await Initialize();

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var parts = message.Split('|', 2);
                    Console.WriteLine($@"[Listener] Nachricht erhalten: {message}");

                    // Make sure parsing worked, else return
                    if (parts.Length != 2)
                    {
                        Console.WriteLine(@"Fehler: Ungültige Nachricht empfangen.");
                        return;
                    }

                    // Extract text, return if empty text
                    var id = parts[0];
                    var extractedText = parts[1];
                    if (string.IsNullOrEmpty(extractedText))
                    {
                        Console.WriteLine($@"Fehler: Leerer OCR-Text für Task {id}. Nachricht wird ignoriert.");
                        return;
                    }

                    // Fetch documentItem from DAL, return if unsuccessful
                    var client = _httpClientFactory.CreateClient("DocumentDAL");
                    var response = await client.GetAsync($"/api/document/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($@"Fehler beim Abrufen des Dokuments mit ID {id}: {response.StatusCode}");
                        return;
                    }

                    var documentItem = await response.Content.ReadFromJsonAsync<DocumentItem>();
                    if (documentItem == null)
                    {
                        Console.WriteLine($@"[Listener] Dokument mit Id {id} nicht gefunden.");
                        return;
                    }

                    Console.WriteLine($@"[Listener] Document {id} erfolgreich abgerufen.");
                    Console.WriteLine($@"[Listener] OCR Text für Document {id}: {extractedText}");
                    Console.WriteLine($@"[Listener] Document vor Update: {documentItem.Id}, {documentItem.Title}, {documentItem.Author}");

                    documentItem.OcrText = extractedText;

                    // Send to MessageQueue Service which submits entire Document including OCR for ElasticSearch Indexing Worker
                    await _messageQueueService.SendToIndexingQueue(documentItem);


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