using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using RTDicomViewer.View.Dialogs;
using RTDicomViewer.ViewModel;
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

namespace RTDicomViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DispatcherHelper.Initialize();
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IProgressView>().Owner = this;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            MainViewModel dc = (MainViewModel)DataContext;
            dc.OnClose();
        }
    }
}
