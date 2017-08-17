using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class SelectableObject<T>
    {
        public bool IsSelected { get { return _isSelected; }
            set { _isSelected = value; if (value) OnSelected(); if (!value) OnUnselected(); } }
        private bool _isSelected;
        public T Value;
        public event EventHandler<SelectableObjectEventArgs> ObjectSelectionChanged;

        public SelectableObject(T value)
        {
            Value = value;
        }

        private void OnUnselected()
        {
            ObjectSelectionChanged?.Invoke(this, new SelectableObjectEventArgs(false));
        }

        private void OnSelected()
        {
            ObjectSelectionChanged?.Invoke(this, new SelectableObjectEventArgs(true));
        }
    }
}
