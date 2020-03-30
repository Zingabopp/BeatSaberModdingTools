using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberModdingTools.BuildTools
{
    public class ReadOnlyList<T> : IReadOnlyList<T>, IList<T>
    {
        public static ReadOnlyList<T> Empty { get; } = new ReadOnlyList<T>(Array.Empty<T>());
        private T[] _items = Array.Empty<T>();
        public T this[int index] => _items[index];

        public int Count => _items.Length;

        public IEnumerator<T> GetEnumerator()
        {

            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }


        public bool IsReadOnly => true;

        T IList<T>.this[int index] { get => _items[index]; set => throw new NotSupportedException("This operation is not supported on a ReadOnlyList"); }

        public int IndexOf(T item)
        {
            bool itemIsNull = item == null;
            for(int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    if (itemIsNull)
                        return i;
                }
                else if (_items[i].Equals(item))
                    return i;
            }
            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException("This operation is not supported on a ReadOnlyList");
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException("This operation is not supported on a ReadOnlyList");
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException("This operation is not supported on a ReadOnlyList");
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException("This operation is not supported on a ReadOnlyList");
        }

        public bool Contains(T item)
        {
            bool itemIsNull = item == null;
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    if (itemIsNull)
                        return true;
                }
                else if (_items[i].Equals(item))
                    return true;
            }
            return false;
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException("This operation is not supported on a ReadOnlyList");
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public ReadOnlyList(T[] items)
        {
            if(items.Length == 0)
            {
                _items = Array.Empty<T>();
                return;
            }
            _items = new T[items.Length];
            items.CopyTo(_items, 0);
        }
    }
}
