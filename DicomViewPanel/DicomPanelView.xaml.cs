﻿using DicomPanel.Core;
using RT.Core.Utilities.RTMath;
using SharpGL;
using SharpGL.SceneGraph;
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

        public bool SpyGlassChecked { get { return Model.ToolBox.ActivatedTools.Contains(Model.ToolBox.GetTool("spyglass")); } set { } }

        public DicomPanelView()
        {
            InitializeComponent();
            DicomPanelViewModel vm = (DicomPanelViewModel)DataContext;
            vm.Canvas = OverlayCanvas;
            vm.OnSizeChanged(this.ActualWidth, this.ActualHeight);
            this.Loaded += DicomPanelView_Loaded;

            this.SizeChanged += WorldViewControl_SizeChanged;
        }

        private void DicomPanelView_Loaded(object sender, RoutedEventArgs e)
        {
            DicomPanelViewModel vm = (DicomPanelViewModel)DataContext;
            vm.OnSizeChanged(this.ActualWidth, this.ActualHeight);
        }

        private void WorldViewControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var dc = (DicomPanelViewModel)DataContext;
            dc.OnSizeChanged(e.NewSize.Width, e.NewSize.Height);
        }

        private void DicomPanelView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                Model?.OnMouseDown(getWorldPoint(e.GetPosition(this)));
        }

        private void DicomPanelView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Released)
                Model?.OnMouseUp(getWorldPoint(e.GetPosition(this)));
        }

        private void DicomPanelView_MouseMove(object sender, MouseEventArgs e)
        {
            Model?.OnMouseMove(getWorldPoint(e.GetPosition(this)));
        }

        private void DicomPanelView_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Model?.OnMouseScroll(getWorldPoint(e.GetPosition(this)), e.Delta);
        }

        private Point3d getWorldPoint(Point pt)
        {
            var t = new CoordinateTransform();
            var x = t.X2Ndc(pt.X, ActualWidth);
            var y = t.Y2Ndc(pt.Y, ActualHeight);
            var worldPoint = Model?.Camera.ConvertScreenToWorldCoords(y, x);
            return worldPoint;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Model?.OnMouseExit(getWorldPoint(e.GetPosition(this)));
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Model?.OnMouseEnter(getWorldPoint(e.GetPosition(this)));
        }

        private void ContextMenuToolClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            Model.ToolBox.SelectTool(menuItem.Tag.ToString());
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(1, 0, 0);
            gl.Vertex(0, 0);
            gl.Vertex(1, 1);
            //((DicomPanelViewModel)(this.DataContext)).Draw()
        }
    }
}
