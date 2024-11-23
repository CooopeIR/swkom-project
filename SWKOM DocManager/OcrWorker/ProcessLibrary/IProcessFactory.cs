using System.Diagnostics;

namespace OCRWorker.ProcessLibrary
{
    public interface IProcessFactory
    {
        IProcess CreateProcess(string fileName, string arguments);
    }

    public class ProcessFactory : IProcessFactory
    {
        private readonly IProcessStartInfoFactory _psiFactory;
        public ProcessFactory(IProcessStartInfoFactory psifactory)
        {
            _psiFactory = psifactory;
        }

        public IProcess CreateProcess(string fileName, string arguments)
        {
            ProcessStartInfo psi = _psiFactory.Create(fileName, arguments);

            return new ProcessWrapper(psi);
        }
    }
}
