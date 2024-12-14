using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    /// <summary>
    /// Interface IProcessStartInfoFactory for ProcessStartInfoFactory;
    /// Class with function to create ProcessStartInfo element and returns it
    /// </summary>
    public interface IProcessStartInfoFactory
    {
        /// <summary>
        /// Interface; Create new ProcessStartInfo, fills it with given values 
        /// (additional RedirectStandardOutput and CreateNoWindow =true, UseShellExecute=false) and returns it
        /// </summary>
        /// <param name="fileName">string fileName</param>
        /// <param name="arguments">string arguments</param>
        /// <returns>Created and filled ProcessStartInfo</returns>
        ProcessStartInfo Create(string fileName, string arguments);
    }

    /// <summary>
    /// Class with function to create ProcessStartInfo element and returns it
    /// </summary>
    public class ProcessStartInfoFactory : IProcessStartInfoFactory
    {
        /// <summary>
        /// Create new ProcessStartInfo, fills it with given values 
        /// (additional RedirectStandardOutput and CreateNoWindow =true, UseShellExecute=false) and returns it
        /// </summary>
        /// <param name="fileName">string fileName</param>
        /// <param name="arguments">string arguments</param>
        /// <returns>Created and filled ProcessStartInfo</returns>
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
