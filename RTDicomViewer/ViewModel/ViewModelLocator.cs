/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:RTDicomViewer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RTDicomViewer.IO;
using RTDicomViewer.Utilities;
using RTDicomViewer.View.Dialogs;
using RTDicomViewer.ViewModel.Dialogs;
using RTDicomViewer.ViewModel.MainWindow;
using RTDicomViewer.ViewModel.MainWindow.UtilityView;

namespace RTDicomViewer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<DoseObjectDisplayViewModel>();
            SimpleIoc.Default.Register<ROIObjectDisplayViewModel>();
            SimpleIoc.Default.Register<POIObjectDisplayViewModel>();
            SimpleIoc.Default.Register<ImageObjectDisplayViewModel>();
            SimpleIoc.Default.Register<AnalyseDisplayViewModel>();
            SimpleIoc.Default.Register<PlanViewModel>();
            SimpleIoc.Default.Register<HistogramCreaterWindowViewModel>();
            SimpleIoc.Default.Register<HistogramViewModel>();
            SimpleIoc.Default.Register<IProgressService, ProgressDialogViewModel>();
            SimpleIoc.Default.Register<IHistogramBuilder, HistogramBuilder>();
            SimpleIoc.Default.Register<IProgressView, ProgressDialogView>();
            SimpleIoc.Default.Register<IFileOpener, FileOpener>();
            SimpleIoc.Default.Register<DVHViewModel>();
            SimpleIoc.Default.Register<DVHObjectDisplayViewModel>();
            SimpleIoc.Default.Register<IDVHBuilder, DVHBuilder>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public DoseObjectDisplayViewModel DoseObjectDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DoseObjectDisplayViewModel>();
            }
        }

        public ROIObjectDisplayViewModel ROIDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ROIObjectDisplayViewModel>();
            }
        }

        public POIObjectDisplayViewModel POIDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<POIObjectDisplayViewModel>();
            }
        }

        public ImageObjectDisplayViewModel ImageDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImageObjectDisplayViewModel>();
            }
        }

        public AnalyseDisplayViewModel AnalyseDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AnalyseDisplayViewModel>();
            }
        }

        public PlanViewModel PlanDisplay
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlanViewModel>();
            }
        }

        public HistogramCreaterWindowViewModel HistogramCreaterWindowViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistogramCreaterWindowViewModel>();
            }
        }

        public HistogramViewModel HistogramViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistogramViewModel>();
            }
        }

        public ProgressDialogViewModel ProgressDialogViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProgressDialogViewModel>();
            }
        }

        public DVHViewModel DVHViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DVHViewModel>();
            }
        }

        public DVHObjectDisplayViewModel DVHObjectDisplayViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DVHObjectDisplayViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}