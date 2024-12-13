using ElasticSearch.Models;
using RabbitMQ.Client;

namespace ElasticSearch
{
    public interface IIndexingWorker
    {
        void ConnectToRabbitMQ();
        IModel GetChannel(); // To access the channel in tests
        public Task IndexDocument(Document item);
        public void Start();
        public void Dispose();
    }
}
