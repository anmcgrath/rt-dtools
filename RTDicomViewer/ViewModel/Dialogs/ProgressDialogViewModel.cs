using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RTDicomViewer.Message;
using RTDicomViewer.View.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public class ProgressDialogViewModel:ViewModelBase,IProgressService
    {
        public ObservableCollection<ProgressItem> ObjectProgressItems { get; set; }
        public IProgressView View;

        public ProgressDialogViewModel(IProgressView view)
        {
            ObjectProgressItems = new ObservableCollection<ProgressItem>();
            View = view;
        }

        public ProgressItem CreateNew(string title, bool isIndeterminate)
        {
            ProgressItem item = new ProgressItem();
            item.Title = title;
            item.IsIndeterminate = isIndeterminate;
            ObjectProgressItems.Add(item);
            View?.Show();
            return item; 
        }

        public void End(ProgressItem item)
        {
            ObjectProgressItems.Remove(item);
            if (ObjectProgressItems.Count == 0)
                View?.Hide();
        }

    }
}
