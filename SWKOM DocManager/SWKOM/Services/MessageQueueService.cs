using DocumentDAL.Entities;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using IConnectionFactory = RabbitMQ.Client.IConnectionFactory;

namespace SWKOM.Services
{
    /// <summary>
    /// Connection management functions for RabbitMQ
    /// </summary>
    public class MessageQueueService : IMessageQueueService, IDisposable
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IConnectionFactory _connectionFactory;

        /// <summary>
        /// IConnection Connection to set and get private _connection variable
        /// </summary>
        public IConnection Connection => _connection;


        /// <summary>
        /// Create connection with connection details for RabbitMQ (file queue)
        /// </summary>
        //public MessageQueueService(IConnection? connection = null, IModel? channel = null)
        //{
        //    var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" };
        //    if (connection != null)
        //    {
        //        _connection = connection;
        //    }
        //    else
        //    {
        //        _connection = factory.CreateConnection();
        //    }
        //    if (channel != null)
        //    {
        //        _channel = channel;
        //    }
        //    else
        //    {
        //        _channel = _connection.CreateModel();
        //    }
        //    _channel.QueueDeclare(queue: "file_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        //    _channel.QueueDeclare(queue: "indexing_queue", durable: false, exclusive: false, autoDelete: false,
        //        arguments: null);
        //}

        public MessageQueueService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            ConnectToRabbitMQ();
        }

        /// <summary>
        /// Tries to open a connection to RabbitMQ, creates queues for ocr_result_queue and indexing_queue
        /// </summary>
        /// <returns>Task.CompletedTask</returns>
        /// <exception cref="Exception">Connection failure with RabbitMQ</exception>
        public Task ConnectToRabbitMQ()
        {
            int retries = 15;
            while (retries > 0)
            {
                try
                {
                    _connection = _connectionFactory.CreateConnection();

                    _channel = _connection.CreateModel();

                    // Queue to consume OCR Results
                    _channel.QueueDeclare(queue: "ocr_result_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    // Queue to publish DocumentItems for Indexing Worker
                    _channel.QueueDeclare(queue: "indexing_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    Console.WriteLine("[MsgService] Erfolgreich mit RabbitMQ verbunden und Queue erstellt.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MsgService] Fehler beim Verbinden mit RabbitMQ: {ex.Message}. Versuche es in 5 Sekunden erneut...");
                    Thread.Sleep(5000);
                    retries--;
                }
            }
            if (_connection == null || !_connection.IsOpen)
            {
                throw new Exception("Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen.");
            }
            return Task.CompletedTask;
        }




        /// <summary>
        /// Send message to RabbitMQ file queue
        /// </summary>
        /// <param name="message">type: string</param>
        public void SendToFileQueue(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "file_queue", basicProperties: null, body: body);
            Console.WriteLine($"[x] Sent {message}");
        }

        /// <summary>
        /// Send DocumentItem to Indexing queue
        /// </summary>
        /// <param name="item"></param>
        public void SendToIndexingQueue(DocumentItem item)
        {
            var documentJson = JsonSerializer.Serialize(item);
            var body = Encoding.UTF8.GetBytes(documentJson);
            _channel.BasicPublish(exchange: "", routingKey: "indexing_queue", basicProperties: null, body: body);
            Console.WriteLine($"[x] Sent Document with ID {item.Id} to Indexing Service");
        }

        /// <summary>
        /// Close connection to RabbitMQ
        /// </summary>
        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
            }
            if (_connection.IsOpen)
            {
                _connection.Close();
            }
        }
    }
}