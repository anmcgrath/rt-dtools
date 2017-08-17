using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class SelectableObjectEventArgs
    {
        public bool IsNowSelected;
        public SelectableObjectEventArgs(bool isNowSelected)
        {
            IsNowSelected = isNowSelected;
        }
    }
}
