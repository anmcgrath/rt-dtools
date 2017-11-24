using RTDicomViewer.ViewModel.MainWindow;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDicomViewer.View.MainWindow
{
    /// <summary>
    /// Interaction logic for PlanView.xaml
    /// </summary>
    public partial class PlanView : UserControl
    {
        public PlanView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ((PlanViewModel)DataContext).OnBeamUpdated();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ((PlanViewModel)DataContext).OnBeamUpdated();
        }
    }
}
