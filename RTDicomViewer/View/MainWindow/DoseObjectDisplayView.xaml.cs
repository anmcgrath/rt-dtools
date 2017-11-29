using RTDicomViewer.Utilities;
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
    /// Interaction logic for DoseObjectDisplayView.xaml
    /// </summary>
    public partial class DoseObjectDisplayView : UserControl
    {
        public DoseObjectDisplayView()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var vm = (DoseObjectDisplayViewModel)DataContext;
            var wrapper = (DoseGridWrapper)((CheckBox)sender).Tag;
            vm.OnRenderDoseChanged(wrapper);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = (DoseObjectDisplayViewModel)DataContext;
            var wrapper = (DoseGridWrapper)((ComboBox)sender).Tag;
            vm.OnLUTTypeChanged(wrapper);
        }
    }
}
