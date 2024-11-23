using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        public ProcessWrapper(ProcessStartInfo startInfo)
        {
            _process = new Process { StartInfo = startInfo };
        }

        // For reading standard output, for example
        public string GetOutput()
        {
            return _process.StandardOutput.ReadToEnd();
        }

        public StreamReader StandardOutput => _process.StandardOutput;

        public ProcessStartInfo StartInfo
        {
            get => _process.StartInfo;
            set => _process.StartInfo = value;
        }

        public bool Start()
        {
            return _process.Start();
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
        }

        // Ensure process is killed when done
        public void Kill()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }

    }
}
