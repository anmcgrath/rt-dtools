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

namespace RTDicomViewer.View.CustomControls
{
    /// <summary>
    /// Interaction logic for TileControl.xaml
    /// </summary>
    public partial class TileControl : UserControl
    {
        public TileControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TopLeftProperty =
        DependencyProperty.RegisterAttached("TopLeft", typeof(TilerControlItem), typeof(TileControl), new PropertyMetadata(default(TilerControlItem)));

        public static void SetTopLeft(UIElement element, TilerControlItem value)
        {
            element.SetValue(TopLeftProperty, value);
        }

        public static TilerControlItem GetTopLeft(UIElement element)
        {
            return (TilerControlItem)element.GetValue(TopLeftProperty);
        }

        public static readonly DependencyProperty TopRightProperty =
DependencyProperty.RegisterAttached("TopRight", typeof(TilerControlItem), typeof(TileControl), new PropertyMetadata(default(TilerControlItem)));

        public static void SetTopRight(UIElement element, TilerControlItem value)
        {
            element.SetValue(TopRightProperty, value);
        }

        public static TilerControlItem GetTopRight(UIElement element)
        {
            return (TilerControlItem)element.GetValue(TopRightProperty);
        }

        public static readonly DependencyProperty BottomLeftProperty =
DependencyProperty.RegisterAttached("BottomLeft", typeof(TilerControlItem), typeof(TileControl), new PropertyMetadata(default(TilerControlItem)));

        public static void SetBottomLeft(UIElement element, TilerControlItem value)
        {
            element.SetValue(BottomLeftProperty, value);
        }

        public static TilerControlItem GetBottomLeft(UIElement element)
        {
            return (TilerControlItem)element.GetValue(BottomLeftProperty);
        }

        public static readonly DependencyProperty BottomRightProperty =
DependencyProperty.RegisterAttached("BottomRight", typeof(TilerControlItem), typeof(TileControl), new PropertyMetadata(default(TilerControlItem)));

        public static void SetBottomRight(UIElement element, TilerControlItem value)
        {
            element.SetValue(BottomRightProperty, value);
        }

        public static TilerControlItem GetBottomRight(UIElement element)
        {
            return (TilerControlItem)element.GetValue(BottomRightProperty);
        }
    }
}
