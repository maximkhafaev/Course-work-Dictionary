﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KTrie
{
    public class Trie<TKey, TValue> : IDictionary<IEnumerable<TKey>, TValue>
    {
        private readonly TrieSet<TKey> _trie;

        public Trie() : this(EqualityComparer<TKey>.Default)
        {

        }

        public Trie(IEqualityComparer<TKey> comparer)
        {
            _trie = new TrieSet<TKey>(comparer);
        }

        public int Count => _trie.Count;

        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.IsReadOnly => false;

        public ICollection<IEnumerable<TKey>> Keys => _trie.ToList();

        public ICollection<TValue> Values => _trie.Cast<TrieEntryPrivate>().Select(te => te.Value).ToArray();

        public TValue this[IEnumerable<TKey> key]
        {
            get
            {
                if (TryGetValue(key, out var val))
                {
                    return val;
                }

                throw new KeyNotFoundException($"The given key was not present in the trie.");
            }
            set
            {
                var result = _trie.TryGetItem(key, out var trieEntry);

                if (result)
                {
                    ((TrieEntryPrivate)trieEntry).Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public IEnumerable<TrieEntry<TKey, TValue>> GetByPrefix(IEnumerable<TKey> prefix) => 
            _trie.GetByPrefix(prefix).Cast<TrieEntryPrivate>().Select(i => new TrieEntry<TKey, TValue>(i, i.Value));


        public IEnumerator<KeyValuePair<IEnumerable<TKey>, TValue>> GetEnumerator() => 
            _trie.Cast<TrieEntryPrivate>().Select(i => new KeyValuePair<IEnumerable<TKey>, TValue>(i, i.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Add(KeyValuePair<IEnumerable<TKey>, TValue> item) =>
            Add(item.Key, item.Value);

        public void Clear() => _trie.Clear();


        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Contains(KeyValuePair<IEnumerable<TKey>, TValue> item)
        {
            var result = _trie.TryGetItem(item.Key, out var trieEntry);

            if (result)
            {
                var value = ((TrieEntryPrivate)trieEntry).Value;

                if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(KeyValuePair<IEnumerable<TKey>, TValue>[] array, int arrayIndex) =>
            Array.Copy(_trie.Cast<TrieEntryPrivate>().Select(i => new KeyValuePair<IEnumerable<TKey>, TValue>(i, i.Value)).ToArray(), 0, array, arrayIndex, Count);

        bool ICollection<KeyValuePair<IEnumerable<TKey>, TValue>>.Remove(KeyValuePair<IEnumerable<TKey>, TValue> item)
        {
            var result = _trie.TryGetItem(item.Key, out var trieEntry);

            if (result)
            {
                var value = ((TrieEntryPrivate) trieEntry).Value;

                if (EqualityComparer<TValue>.Default.Equals(item.Value, value))
                {
                    return Remove(item.Key);
                }
            }

            return false;
        }

        public bool ContainsKey(IEnumerable<TKey> key) => _trie.Contains(key);

        public void Add(IEnumerable<TKey> key, TValue value) => 
            _trie.Add(new TrieEntryPrivate(key) { Value = value });

        public bool Remove(IEnumerable<TKey> key) => _trie.Remove(key);

        public bool TryGetValue(IEnumerable<TKey> key, out TValue value)
        {
            var result = _trie.TryGetItem(key, out var trieEntry);

            value = result ? ((TrieEntryPrivate)trieEntry).Value : default;

            return result;
        }

        private sealed class TrieEntryPrivate : IEnumerable<TKey>
        {
            public TrieEntryPrivate(IEnumerable<TKey> key)
            {
                Key = key;
            }

            private IEnumerable<TKey> Key { get; }

            public TValue Value { get; set; }

            public IEnumerator<TKey> GetEnumerator() => Key.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}