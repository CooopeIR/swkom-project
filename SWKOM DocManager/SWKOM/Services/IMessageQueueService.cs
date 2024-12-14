using DocumentDAL.Entities;
using RabbitMQ.Client;

namespace SWKOM.Services
{
    /// <summary>
    /// Interface IMessageQueueService for MessageQueueService;
    /// Connection management functions for RabbitMQ
    /// </summary>
    public interface IMessageQueueService
    {
        public IConnection Connection { get; }

        public Task ConnectToRabbitMQ();

        /// <summary>
        /// Interface: Send message to RabbitMQ file queue
        /// </summary>
        /// <param name="message">type: string</param>
        void SendToFileQueue(string message);
        /// <summary>
        /// Interface: Send DocumentItem to Indexing queue
        /// </summary>
        /// <param name="item"></param>
        void SendToIndexingQueue(DocumentItem item);
    }
}