using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class RTObjectLoadedMessage<T>
    {
        public T Value { get; set; }
        public RTObjectLoadedMessage(T value)
        {
            Value = value;
        }
    }
}
