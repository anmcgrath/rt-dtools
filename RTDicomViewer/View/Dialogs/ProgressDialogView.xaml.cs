using GalaSoft.MvvmLight.Ioc;
using RTDicomViewer.ViewModel.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RTDicomViewer.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ProgressDialogView.xaml
    /// </summary>
    public partial class ProgressDialogView : Window, IProgressView
    {

        public ProgressDialogView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
