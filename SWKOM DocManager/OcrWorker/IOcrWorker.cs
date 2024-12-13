using RabbitMQ.Client;

namespace OCRWorker
{
    public interface IOcrWorker
    {
        void ConnectToRabbitMQ();
        IModel GetChannel(); // To access the channel in tests
        public string PerformOcr(string filePath);
        public void Start();
        public void Dispose();
    }
}
