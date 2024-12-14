using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    /// <summary>
    /// ProcessWrapper Class with functions for easier access
    /// </summary>
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process;

        /// <summary>
        /// Constructor for ProcessWrapper class
        /// </summary>
        /// <param name="startInfo">Gets ProcessStartInfo startinfo, makes new Process with given startInfo</param>
        public ProcessWrapper(ProcessStartInfo startInfo)
        {
            _process = new Process { StartInfo = startInfo };
        }

        /// <summary>
        /// For reading standard output, for example
        /// </summary>
        /// <returns>read from current position to end and returns it</returns>
        public string GetOutput()
        {
            return _process.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Wrapper for _process.StandardOutput
        /// </summary>
        public StreamReader StandardOutput => _process.StandardOutput;

        /// <summary>
        /// Getter and Setter for _process.StartInfo
        /// </summary>
        public ProcessStartInfo StartInfo
        {
            get => _process.StartInfo;
            set => _process.StartInfo = value;
        }

        /// <summary>
        /// Start process with _process.Start
        /// </summary>
        /// <returns>true if process is started or false if no new process is started</returns>
        public bool Start()
        {
            return _process.Start();
        }

        /// <summary>
        /// Call function _process.WaitForExit to wait for process to exit
        /// </summary>
        public void WaitForExit()
        {
            _process.WaitForExit();
        }

        /// <summary>
        /// Ensure process is killed when done
        /// </summary>
        public void Kill()
        {
            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }

    }
}
