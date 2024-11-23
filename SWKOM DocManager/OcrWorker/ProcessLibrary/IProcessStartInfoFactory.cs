using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    public interface IProcessStartInfoFactory
    {
        ProcessStartInfo Create(string fileName, string arguments);
    }


    public class ProcessStartInfoFactory : IProcessStartInfoFactory
    {
        public ProcessStartInfo Create(string fileName, string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }
    }
}
