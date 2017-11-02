using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class ProgressMessage
    {
        public Progress ProgressType { get; set; }
        public double ProgressValue { get; set; }
        public bool IsIndeterminate { get; set; }
        public object Sender { get; set; }
        public string Title { get; set; }
        public ProgressMessage(object sender, Progress progressType, double progressValue, bool isIndeterminate, string title)
        {
            ProgressType = progressType;
            ProgressValue = progressValue;
            IsIndeterminate = isIndeterminate;
            Sender = sender;
            Title = title;
        }

        public ProgressMessage(object sender, Progress progressType, double progressValue, string title)
            : this(sender, progressType, progressValue, false, title) { }
        public ProgressMessage(object sender, Progress progressType, string title) : this(sender, progressType, 0, true, title) { }
        public ProgressMessage(object sender, string title) : this(sender, Progress.Begin, 0, false, title) { }
    }

    public enum Progress
    {
        Begin,
        Progress,
        End,
    }
}
