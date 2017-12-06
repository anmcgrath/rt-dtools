using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public interface IProgressService
    {
        ProgressItem CreateNew(string title, bool isIndeterminate);
        void End(ProgressItem progressItem);
    }
}
