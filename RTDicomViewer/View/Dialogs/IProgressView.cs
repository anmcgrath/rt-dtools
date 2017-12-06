using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RTDicomViewer.ViewModel.Dialogs;

namespace RTDicomViewer.View.Dialogs
{
    public interface IProgressView
    {
        void Show();
        void Hide();
        Window Owner { get; set; }
        object DataContext { get; set; }
    }
}
