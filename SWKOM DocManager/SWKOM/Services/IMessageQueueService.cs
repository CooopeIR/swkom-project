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
        /// <summary>
        /// IConnection Connection to set and get private _connection variable
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Interface: Tries to open a connection to RabbitMQ, creates queues for ocr_result_queue and indexing_queue
        /// </summary>
        /// <returns>Task.CompletedTask</returns>
        /// <exception cref="Exception">Connection failure with RabbitMQ</exception>
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