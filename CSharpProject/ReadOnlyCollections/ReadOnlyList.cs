using System;
using System.Collections.Generic;

namespace Windsmoon.ReadOnlyCollections
{
    public struct ReadOnlyList<T>
    {
        #region fields
        private readonly List<T> _list;
        #endregion

        #region constructors
        public ReadOnlyList(List<T> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }
        #endregion

        #region indexers
        public T this[int index] => _list[index];
        #endregion
        
        #region properties
        public int Count => _list.Count;
        public bool IsEmpty => _list.Count == 0;
        #endregion

        #region methods
        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();
        public bool Contains(T item) => _list.Contains(item);
        public bool Exists(Predicate<T> match) => _list.Exists(match);
        public T FindFirst(Predicate<T> match) => _list.Find(match);
        public T FindLast(Predicate<T> match) => _list.FindLast(match);
        public int IndexOf(T item) => _list.IndexOf(item);
        public int FindIndex(Predicate<T> match) => _list.FindIndex(match);
        public int FindLastIndex(Predicate<T> match) => _list.FindLastIndex(match);
        public void CopyTo(T[] array) => _list.CopyTo(array);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public void CopyTo(int index, T[] array, int arrayIndex, int count) => _list.CopyTo(index, array, arrayIndex, count);
        #endregion
    }
}
