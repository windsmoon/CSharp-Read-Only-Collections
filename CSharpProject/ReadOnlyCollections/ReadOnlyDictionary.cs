using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Windsmoon.ReadOnlyCollections
{
    public struct ReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        #region fields
        private readonly Dictionary<TKey, TValue> _dictionary;
        #endregion

        #region constructors
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }
        #endregion

        #region indexers
        public TValue this[TKey key] => _dictionary[key];
        #endregion

        #region properties
        public int Count => _dictionary.Count;
        public bool IsEmpty => _dictionary.Count == 0;
        public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;
        #endregion

        #region methods
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => _dictionary.GetEnumerator();
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool ContainsValue(TValue value) => _dictionary.ContainsValue(value);
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => _dictionary.TryGetValue(key, out value);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array) => CopyTo(array, 0);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        #endregion
    }
}
