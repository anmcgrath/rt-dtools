using RTDicomViewer.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.ViewModel.Dialogs
{
    public class ProgressDialogViewModel
    {
        public ObservableCollection<ObjectProgressStatus> ObjectProgressStatuses { get; set; }

        public ProgressDialogViewModel()
        {
            ObjectProgressStatuses = new ObservableCollection<ObjectProgressStatus>();
        }

        public void Apply(ProgressMessage msg)
        {
            var ops = ObjectProgressStatuses.Where((x) => { return x.Object == msg.Sender; }).FirstOrDefault();
            if(ops == null)
            {
                if(msg.ProgressType == Progress.Begin)
                {
                    var newOps = new ObjectProgressStatus(msg.Sender);
                    newOps.IsIndeterminate = msg.IsIndeterminate;
                    newOps.CumulativeProgressAmount = msg.ProgressValue;
                    newOps.Title = msg.Title;
                    ObjectProgressStatuses.Add(newOps);
                }
            }else
            {
                if(msg.ProgressType == Progress.End)
                {
                    ObjectProgressStatuses.Remove(ops);
                    ops = null;
                }else if(msg.ProgressType == Progress.Progress)
                {
                    ops.CumulativeProgressAmount = msg.ProgressValue;
                    ops.Title = msg.Title;
                    ops.IsIndeterminate = msg.IsIndeterminate;
                }
            }
        }
    }

    public class ObjectProgressStatus
    {
        public object Object;
        public double CumulativeProgressAmount { get; set; }
        public string Title { get; set; }
        public bool IsIndeterminate { get; set; }
        public ObjectProgressStatus(object obj)
        {
            Object = obj;
        }
    }
}
