using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public class ProgressItem:ViewModelBase
    {
        public string Title { get; set; }
        public bool IsIndeterminate { get; set; }
        public int ProgressAmount
        { get { return _progressAmount; } set { _progressAmount = value; RaisePropertyChanged("ProgressAmount"); } }

        private int _progressAmount;

        public ProgressItem() { }
    }
}
