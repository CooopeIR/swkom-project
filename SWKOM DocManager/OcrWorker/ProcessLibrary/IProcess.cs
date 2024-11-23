using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    // This interface is required to abstract the Process Class for Mocking purposes. Without it, the Unit Testing would be difficult
    public interface IProcess
    {
        StreamReader StandardOutput { get; }
        ProcessStartInfo StartInfo { get; set; }
        public string GetOutput();

        bool Start();
        void WaitForExit();
        void Kill();
    }
}
