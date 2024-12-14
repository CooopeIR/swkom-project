using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    /// <summary>
    /// This interface is required to abstract the Process Class for Mocking purposes. Without it, the Unit Testing would be difficult
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Wrapper for _process.StandardOutput
        /// </summary>
        StreamReader StandardOutput { get; }
        /// <summary>
        /// Getter and Setter for _process.StartInfo
        /// </summary>
        ProcessStartInfo StartInfo { get; set; }
        /// <summary>
        /// For reading standard output, for example
        /// </summary>
        /// <returns>read from current position to end and returns it</returns>
        public string GetOutput();

        /// <summary>
        /// Start process with _process.Start
        /// </summary>
        /// <returns>true if process is started or false if no new process is started</returns>
        bool Start();
        /// <summary>
        /// Call function _process.WaitForExit to wait for process to exit
        /// </summary>
        void WaitForExit();
        /// <summary>
        /// Ensure process is killed when done
        /// </summary>
        void Kill();
    }
}
