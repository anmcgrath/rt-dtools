using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Utilities
{
    public class SelectableObject<T>:SelectableObject<T,string>
    {
        public SelectableObject(T value) : base(value) { }
    }

    public class SelectableObject<T,C>:INotifyPropertyChanged
    {
        /// <summary>
        /// Whether or not to fire a selction event when selected
        /// </summary>
        public bool FireSelectionEvent { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value;
                if (value) OnSelected();
                if (!value) OnUnselected();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }
        private bool _isSelected;

        public T Value { get; set; }
        public event EventHandler<SelectableObjectEventArgs> ObjectSelectionChanged;
        public event EventHandler<MultiSelectableObjectEventArgs<C>> ChildrenObjectsSelectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SelectableObject<C>> Children { get; set; }

        public SelectableObject(T value)
        {
            FireSelectionEvent = true;
            Value = value;
            Children = new ObservableCollection<SelectableObject<C>>();
        }

        private void OnUnselected()
        {
            if (FireSelectionEvent)
            {
                List<SelectableObject<C>> unselectedChildren = new List<SelectableObject<C>>();
                foreach (var child in Children)
                {
                    child.FireSelectionEvent = false;
                    if (child.IsSelected)
                        unselectedChildren.Add(child);
                    child.IsSelected = false;
                    child.FireSelectionEvent = true;
                }
                ObjectSelectionChanged?.Invoke(this, new SelectableObjectEventArgs(false));
                if (unselectedChildren.Count > 0)
                    ChildrenObjectsSelectionChanged?.Invoke(this, new MultiSelectableObjectEventArgs<C>(new List<SelectableObject<C>>(), unselectedChildren));
            }
        }

        private void OnSelected()
        {
            if (FireSelectionEvent)
            {
                List<SelectableObject<C>> selectedChildren = new List<SelectableObject<C>>();
                foreach (var child in Children)
                {
                    child.FireSelectionEvent = false;
                    if (!child.IsSelected)
                        selectedChildren.Add(child);

                    child.IsSelected = true;
                    child.FireSelectionEvent = true;
                }

                ObjectSelectionChanged?.Invoke(this, new SelectableObjectEventArgs(true));
                if (selectedChildren.Count > 0)
                    ChildrenObjectsSelectionChanged?.Invoke(this, new MultiSelectableObjectEventArgs<C>(selectedChildren, new List<SelectableObject<C>>()));
            }
        }

        public void AddChild(SelectableObject<C> child)
        {
            child.ObjectSelectionChanged += Child_ObjectSelectionChanged;
            Children.Add(child);
        }

        public void RemoveChild(SelectableObject<C> child)
        {
            child.ObjectSelectionChanged -= Child_ObjectSelectionChanged;
            Children.Remove(child);
        }

        private void Child_ObjectSelectionChanged(object sender, SelectableObjectEventArgs e)
        {
            List<SelectableObject<C>> selectedChildren = new List<SelectableObject<C>>();
            List<SelectableObject<C>> unselectedChildren = new List<SelectableObject<C>>();

            var childSelection = (SelectableObject<C>)sender;

            if(!e.IsNowSelected)
            {
                unselectedChildren.Add(childSelection);
                if (IsSelected)
                {
                    FireSelectionEvent = false;
                    IsSelected = false;
                    FireSelectionEvent = true;
                }
            }else
            {
                selectedChildren.Add(childSelection);

                bool allChildrenSelected = true;
                foreach(var child in Children)
                {
                    if (!child.IsSelected)
                    {
                        allChildrenSelected = false;
                        break;
                    }
                }
                if(allChildrenSelected)
                {
                    FireSelectionEvent = false;
                    IsSelected = true;
                    FireSelectionEvent = true;
                }
            }

            this.ChildrenObjectsSelectionChanged?.Invoke(this, new MultiSelectableObjectEventArgs<C>(selectedChildren, unselectedChildren));          
        }
    }
}
