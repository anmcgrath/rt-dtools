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
    /// Interaction logic for LUTController.xaml
    /// </summary>
    public partial class LUTController : UserControl
    {
        public LUTController()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        
        public double UpperValue
        {
            get { return (double)GetValue(UpperValueProperty); }
            set { if (value > Max) value = Max;
                if (value < Min) value = Min;
                SetValue(UpperValueProperty, value);
                Canvas.SetTop(this.topTriangleShape, canvas.Height - canvas.Height * (value - Min) / (Max - Min));
            }
        }

        // Using a DependencyProperty as the backing store for UpperValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(LUTController), new PropertyMetadata(0.0, OnUpperValuePropertyChanged));

        public static void OnUpperValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                ((LUTController)d).UpperValue = (double)e.NewValue;
        }

        public double LowerValue
        {
            get { return (double)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LowerValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(LUTController), new PropertyMetadata(0.0));



        public double Max
        {
            get { return (double)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Max.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(double), typeof(LUTController), new PropertyMetadata(255.0));



        public double Min
        {
            get { return (double)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Min.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(double), typeof(LUTController), new PropertyMetadata(0.0));

        private bool topTriangleMouseDown = false;
        private double offset;
        private void topTriangleShape_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void topTriangleShape_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //topTriangleMouseDown = false;
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            topTriangleMouseDown = false;
            Mouse.Capture(null);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (topTriangleMouseDown)
            {
                var posn = e.GetPosition(this);
                if (posn.Y < 0)
                    posn.Y = 0;
                UpperValue = Max - (Min + ((posn.Y - offset) / canvas.Height) * (Max - Min));
            }
        }

        private void topTriangleShape_MouseDown(object sender, MouseButtonEventArgs e)
        {
            topTriangleMouseDown = true;
            offset = e.GetPosition(this).Y - Canvas.GetTop(this.topTriangleShape);
            Mouse.Capture(canvas);
        }
    }
}
