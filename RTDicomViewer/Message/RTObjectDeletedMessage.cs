using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Message
{
    public class RTObjectDeletedMessage<T>
    {
        public T Value { get; set; }
        public RTObjectDeletedMessage(T value)
        {
            Value = value;
        }
    }
}
