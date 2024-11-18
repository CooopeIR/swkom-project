namespace SWKOM.Services
{
    public interface IMessageQueueService
    {
        void SendToQueue(string message);
    }
}