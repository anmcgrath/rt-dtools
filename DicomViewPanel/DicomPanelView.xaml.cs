using DicomPanel.Core;
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

namespace DicomPanel.Wpf
{
    /// <summary>
    /// Interaction logic for DicomPanelView.xaml
    /// </summary>
    public partial class DicomPanelView : UserControl
    {
        /// <summary>
        /// The model for the DicomPanelView
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
             DependencyProperty.Register("Model", typeof(DicomPanelModel),
             typeof(DicomPanelView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnModelPropertyChanged));
        public DicomPanelModel Model
        {
            get { return (DicomPanelModel)GetValue(ModelProperty); }
            set {SetValue(ModelProperty, value); }
        }
        private static void OnModelPropertyChanged(DependencyObject source,
        DependencyPropertyChangedEventArgs e)
        {
            var panel = source as DicomPanelView;
            ((DicomPanelViewModel)(panel.DataContext)).SetModel((DicomPanelModel)e.NewValue);
        }

        public DicomPanelView()
        {
            InitializeComponent();
            this.SizeChanged += WorldViewControl_SizeChanged;
            this.MouseWheel += DicomPanelView_MouseWheel;
        }

        private void DicomPanelView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.MouseDevice.GetPosition(this);
            //Model?.OnMouseScroll()
        }

        private void WorldViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dc = (DicomPanelViewModel)DataContext;
            dc.OnSizeChanged(e.NewSize.Width, e.NewSize.Height);
        }
    }
}
