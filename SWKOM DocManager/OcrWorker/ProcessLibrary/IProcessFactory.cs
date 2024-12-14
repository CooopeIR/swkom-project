using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    /// <summary>
    /// Interface IProcessFactory for ProcessFactory; 
    /// ProcessFactory Class with function to create a new IProcess; for Unit Testing purposes
    /// </summary>
    public interface IProcessFactory
    {
        /// <summary>
        /// Interface; Uses IProcessStartInfoFactory to set newly created ProcessStartInfo, 
        /// creates and returns new ProcessWrapper with ProcessStartInfo
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns>Created IProcess</returns>
        IProcess CreateProcess(string fileName, string arguments);
    }

    /// <summary>
    /// ProcessFactory Class with function to create a new IProcess; for Unit Testing purposes
    /// </summary>
    public class ProcessFactory : IProcessFactory
    {
        private readonly IProcessStartInfoFactory _psiFactory;
        /// <summary>
        /// Constructor for ProcessFactory class
        /// </summary>
        /// <param name="psifactory">Sets private ProcessStartInfoFactory to given IProcessStartInfoFactory value</param>
        public ProcessFactory(IProcessStartInfoFactory psifactory)
        {
            _psiFactory = psifactory;
        }

        /// <summary>
        /// Uses IProcessStartInfoFactory to set newly created ProcessStartInfo, 
        /// creates and returns new ProcessWrapper with ProcessStartInfo
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns>Created IProcess</returns>
        public IProcess CreateProcess(string fileName, string arguments)
        {
            ProcessStartInfo psi = _psiFactory.Create(fileName, arguments);

            return new ProcessWrapper(psi);
        }
    }
}
