using RabbitMQ.Client;
using System.Text;

namespace SWKOM.Services
{
    /// <summary>
    /// Connection management functions for RabbitMQ
    /// </summary>
    public class MessageQueueService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        /// <summary>
        /// Create connection with connection details for RabbitMQ (file queue)
        /// </summary>
        public MessageQueueService()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq", UserName = "user", Password = "password" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "file_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        /// <summary>
        /// Send message to RabbitMQ file queue
        /// </summary>
        /// <param name="message">type: string</param>
        public void SendToQueue(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "file_queue", basicProperties: null, body: body);
            Console.WriteLine($"[x] Sent {message}");
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