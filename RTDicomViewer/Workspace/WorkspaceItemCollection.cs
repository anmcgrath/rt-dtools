using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDicomViewer.Workspace
{
    /// <summary>
    /// Represents a collection of items T which can be accessed by a string key or by an index
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorkspaceItemCollection<T>
    {
        private Dictionary<string, T> stringCollection;
        private List<T> listCollection;

        public WorkspaceItemCollection()
        {
            stringCollection = new Dictionary<string, T>();
            listCollection = new List<T>();
        }

        public List<T> GetList()
        {
            return listCollection;
        }

        public void Add(T item, string key)
        {
            //this[key] = item;
            listCollection.Add(item);
        }

        public void Add(T item)
        {
            listCollection.Add(item);
        }

        public int IndexOf(T item)
        {
            return listCollection.IndexOf(item);
        }

        public string KeyOf(T item)
        {
            int index = stringCollection.Values.ToList().IndexOf(item);
            if (index != -1)
                return stringCollection.Keys.ToList()[index];
            return null;
        }

        public bool Contains(T item)
        {
            return listCollection.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return stringCollection.ContainsKey(key);
        }

        public void Remove(T item)
        {
            if (this.Contains(item))
            {
                //stringCollection.Remove(stringCollection.First(x => x.Value.Equals(item)).Key);
                listCollection.Remove(item);
            }
        }

        public void RemoveKey(string key)
        {
            T item = this[key];
            if(item != null)
            {
                listCollection.Remove(item);
                stringCollection.Remove(key);
            }
        }
        
        public T this[string key]
        {
            get
            {
                if (stringCollection.ContainsKey(key))
                    return stringCollection[key];
                else
                    return default(T);
            }
            set
            {
                if (!stringCollection.ContainsKey(key))
                    stringCollection.Add(key, value);
            }
        }

        public T this[int index]
        {
            get
            {
                if (listCollection.Count > index && index >= 0)
                    return listCollection[index];
                else
                    return default(T);
            }
        }

        public T Current { get; set; }
    }
}
