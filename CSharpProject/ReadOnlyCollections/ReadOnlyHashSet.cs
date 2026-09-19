using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Windsmoon.ReadOnlyCollections
{
    public struct ReadOnlyHashSet<T>
    {
        #region fields
        private readonly HashSet<T> _set;
        #endregion

        #region constructors
        public ReadOnlyHashSet(HashSet<T> set)
        {
            _set = set ?? throw new ArgumentNullException(nameof(set));
        }
        #endregion

        #region properties
        public int Count => _set.Count;
        public bool IsEmpty => _set.Count == 0;
        #endregion

        #region methods
        public HashSet<T>.Enumerator GetEnumerator() => _set.GetEnumerator();
        public bool Contains(T item) => _set.Contains(item);
        public bool Exists(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            foreach (T item in _set)
            {
                if (match(item))
                {
                    return true;
                }
            }
            return false;
        }

        [return: MaybeNull]
        public T FindFirst(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            foreach (T item in _set)
            {
                if (match(item))
                {
                    return item;
                }
            }
            return default;
        }

        public bool TryGetValue(T equalValue, [MaybeNullWhen(false)] out T actualValue) => _set.TryGetValue(equalValue, out actualValue);
        public bool IsSubsetOf(IEnumerable<T> other) => _set.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<T> other) => _set.IsSupersetOf(other);
        public bool IsProperSubsetOf(IEnumerable<T> other) => _set.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<T> other) => _set.IsProperSupersetOf(other);
        public bool Overlaps(IEnumerable<T> other) => _set.Overlaps(other);
        public bool SetEquals(IEnumerable<T> other) => _set.SetEquals(other);
        public void CopyTo(T[] array) => _set.CopyTo(array);
        public void CopyTo(T[] array, int arrayIndex) => _set.CopyTo(array, arrayIndex);
        public void CopyTo(T[] array, int arrayIndex, int count) => _set.CopyTo(array, arrayIndex, count);
        #endregion
    }
}
