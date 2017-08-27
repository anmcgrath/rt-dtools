using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class MultiSelectableObjectEventArgs<T>
    {
        public List<SelectableObject<T>> SelectedObjects { get; set; }
        public List<SelectableObject<T>> UnselectedObjects { get; set; }

        public MultiSelectableObjectEventArgs(List<SelectableObject<T>> selectedObjects, List<SelectableObject<T>> unselectedObjects)
        {
            SelectedObjects = selectedObjects;
            UnselectedObjects = unselectedObjects;
        }
    }
}
