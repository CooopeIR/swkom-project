﻿namespace SWKOM.Services
{
    /// <summary>
    /// Interface IMessageQueueService for MessageQueueService;
    /// Connection management functions for RabbitMQ
    /// </summary>
    public interface IMessageQueueService
    {
        /// <summary>
        /// Interface: Send message to RabbitMQ file queue
        /// </summary>
        /// <param name="message">type: string</param>
        void SendToQueue(string message);
    }
}