using GalaSoft.MvvmLight;
using RT.Core.Planning;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.MainWindow
{
    public class PlanViewModel:ViewModelBase
    {
        public ObservableCollection<Beam> Beams { get; set; }
        public Beam SelectedBeam { get { return _selectedBeam; } set { _selectedBeam = value; RaisePropertyChanged("SelectedBeam"); } }
        private Beam _selectedBeam;

        public PlanViewModel()
        {
            Beams = new ObservableCollection<Beam>();
            MessengerInstance.Register<RTObjectAddedMessage<Beam>>(this, x => { Beams.Add(x.Value); SelectedBeam = x.Value; });
        }

        public void OnBeamUpdated()
        {
            Workspace.Workspace.Current.Axial.Invalidate();
            Workspace.Workspace.Current.Sagittal.Invalidate();
            Workspace.Workspace.Current.Coronal.Invalidate();
        }
    }
}
