using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RT.Core.Dose;
using RT.Core.Eval;
using RT.Core.Geometry;
using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public class GammaWindowViewModel:ViewModelBase
    {
        public IDoseObject SelectedMathDose1 { get { return _selectedMathDose1; } set { _selectedMathDose1 = value; RaisePropertyChanged("SelectedMathDose1"); RaisePropertyChanged("ComputeCommand"); } }
        private IDoseObject _selectedMathDose1;

        public IDoseObject SelectedMathDose2 { get { return _selectedMathDose2; } set { _selectedMathDose2 = value; RaisePropertyChanged("SelectedMathDose2"); RaisePropertyChanged("ComputeCommand"); } }
        private IDoseObject _selectedMathDose2;

        public bool IsComputing { get { return _isComputing; } set { _isComputing = value; RaisePropertyChanged("ComputeCommand"); } }
        private bool _isComputing;

        public RelayCommand ComputeCommand => new RelayCommand(() =>
        {
            if (!IsComputing)
                SubtractDoses();
        },
        () =>
        { return !IsComputing && SelectedMathDose1 != null && SelectedMathDose2 != null; });

        public RelayCommand<Window> CloseWindowCommand => new RelayCommand<Window>(w =>
        {
            w.Close();
        });

        public double DoseDiffTol { get; set; } = 3;
        public double DtaTol { get; set; } = 3;

        public bool IsAbsolute { get; set; } = true;
        public bool UseGlobalDifference { get; set; } = true;

        public ObservableCollection<IDoseObject> Doses { get; set; }

        private IProgressService ProgressSerice;

        public GammaWindowViewModel(IProgressService progressService)
        {
            ProgressSerice = progressService;
            Doses = new ObservableCollection<IDoseObject>();

            foreach(var dose in Workspace.Workspace.Current.Doses.GetList())
            {
                Doses.Add(dose);
            }

            MessengerInstance.Register<RTObjectAddedMessage<DicomDoseObject>>(this, x => AddDose(x.Value));
            MessengerInstance.Register<RTObjectDeletedMessage<DicomDoseObject>>(this, x => RemoveDose(x.Value));
            MessengerInstance.Register<RTObjectAddedMessage<EgsDoseObject>>(this, x => AddDose(x.Value));
            MessengerInstance.Register<RTObjectDeletedMessage<EgsDoseObject>>(this, x => RemoveDose(x.Value));
        }

        private void AddDose(IDoseObject dose)
        {
            Doses.Add(dose);
        }

        private void RemoveDose(IDoseObject dose)
        {
            Doses.Remove(dose);
        }

        public async void SubtractDoses()
        {
            if (SelectedMathDose1 == null || SelectedMathDose2 == null)
                return;
            DicomDoseObject newDoseObject = new DicomDoseObject();
            var math = new GridMath();

            IsComputing = true;
            var progressItem = ProgressSerice.CreateNew("Performing Gamma Calculation...", false);
            var progress = new Progress<int>(x => { progressItem.ProgressAmount = x; });
            await Task.Run(() =>
            {
                newDoseObject.Grid = math.Gamma(SelectedMathDose1.Grid, SelectedMathDose2.Grid, progress, (float)DtaTol, (float)DoseDiffTol, 10);
            });
            ProgressSerice.End(progressItem);

            newDoseObject.Grid.ValueUnit = Unit.Gamma;
            newDoseObject.Grid.Name = "Gamma Result";
            IsComputing = false;
            MessengerInstance.Send<RTObjectAddedMessage<DicomDoseObject>>(new RTObjectAddedMessage<DicomDoseObject>(newDoseObject));
        }
    }
}
