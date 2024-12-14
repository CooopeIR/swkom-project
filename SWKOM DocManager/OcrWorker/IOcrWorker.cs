using RabbitMQ.Client;

namespace OCRWorker
{
    /// <summary>
    /// Interface IOcrWorker for OcrWorker;
    /// Class with OcrWorker and associated functions
    /// </summary>
    public interface IOcrWorker
    {
        /// <summary>
        /// Tries to connect to RabbitMQ
        /// </summary>
        /// <exception cref="Exception">Konnte keine Verbindung zu RabbitMQ herstellen, alle Versuche fehlgeschlagen</exception>
        void ConnectToRabbitMQ();
        /// <summary>
        /// Getter for the channel (useful for testing); To access the channel in tests
        /// </summary>
        /// <returns>IModel _channel</returns>
        IModel GetChannel();
        /// <summary>
        /// OCR Processing: gets filePath, tries to get text out of document and returns text as string
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Text from documents</returns>
        public string PerformOcr(string filePath);
        /// <summary>
        /// Wrapper for OCR Processing with additional needed pre- and postprocessing
        /// </summary>
        public void Start();
        /// <summary>
        /// Dispose function to close channel and connection
        /// </summary>
        public void Dispose();
    }
}
