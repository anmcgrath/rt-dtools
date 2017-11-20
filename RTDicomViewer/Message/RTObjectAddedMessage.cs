using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class RTObjectAddedMessage<T>
    {
        public T Value { get; set; }
        public RTObjectAddedMessage(T value)
        {
            Value = value;
        }
    }
}
