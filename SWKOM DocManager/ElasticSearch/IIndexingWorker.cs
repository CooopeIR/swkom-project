using ElasticSearch.Models;
using RabbitMQ.Client;

namespace ElasticSearch
{
    /// <summary>
    /// Interface IIndexingWorker for IndexingWorker;
    /// Class with IndexingWorker and associated functions
    /// </summary>
    public interface IIndexingWorker
    {
        /// <summary>
        /// Tries to connect to RabbitMQ
        /// </summary>
        /// <exception cref="Exception">Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen</exception>
        void ConnectToRabbitMQ();
        /// <summary>
        /// Get Channel from private _channel variable; To access the channel in tests
        /// </summary>
        /// <returns>IModel _channel</returns>
        IModel GetChannel();
        /// <summary>
        /// Tries to index the given document
        /// </summary>
        /// <param name="item">get Document item</param>
        public Task IndexDocument(Document item);
        /// <summary>
        /// Wrapper for IndexDocument with additional needed preprocessing and exception catching
        /// </summary>
        public void Start();
        /// <summary>
        /// Dispose function to close channel and connection
        /// </summary>
        public void Dispose();
    }
}
