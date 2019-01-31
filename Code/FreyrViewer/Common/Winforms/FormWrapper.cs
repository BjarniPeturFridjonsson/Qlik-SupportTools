using System.Threading;
using System.Threading.Tasks;
using FreyrViewer.Ui.MdiForms;

namespace FreyrViewer.Common
{
    public class FormWrapper
    {
        public FormWrapper(Task<FrmBaseForm> form, CancellationTokenSource cts)
        {
            FormTask = form;
            Cts = cts;
        }

        public Task<FrmBaseForm> FormTask { get; }
        public CancellationTokenSource Cts { get; }
    }
}
