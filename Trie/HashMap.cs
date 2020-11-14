using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Markup;

namespace Trie
{
    public class HashMap<TKey, TValue> : IDictionary<TKey, TValue>
    {

        List<TValue> Values;
        List<TKey> Keys;
        LinkedList<KeyValuePair<TKey, TValue>>[] buckets;
        int capacity;
        int count;
        public bool readOnly = false;
        const double maxLoad = 0.3;
        double curLoad => (double)count / capacity;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        public int Count => count;

        public bool IsReadOnly => false;

        public TValue this[TKey key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                Add(key, value);
            }
        }
        public HashMap()
        {

            Values = new List<TValue>();
            Keys = new List<TKey>();
            count = 0;
            capacity = 2;
            buckets = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
        }

        private int GetHash(TKey input)
        {
            int i = Math.Abs(input.GetHashCode()) % capacity;

            return i;
        }
        public bool Add(TKey key, TValue value)
        {
            int bucket = GetHash(key);
            if (buckets[bucket] == null)
            {
                buckets[bucket] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            if (ContainsKey(key))
            {
                return false;
            }

            buckets[bucket].AddLast(new KeyValuePair<TKey, TValue>(key, value));
            Keys.Add(key);
            Values.Add(value);
            count++;
            if (curLoad >= maxLoad)
            {
                ReHash();
            }

            return true;
        }

        public TValue GetValue(TKey key)
        {
            if (Keys.Count == 0 || ContainsKey(key) == false)
            {
                throw new KeyNotFoundException("Key is not found.");
            }
            int i = GetHash(key);
            foreach (var node in buckets[i])
            {
                if (node.Key.Equals(key))
                {
                    return node.Value;
                }
            }
            throw new KeyNotFoundException("The key you are looking for is not found.");
        }
        public void ReHash()
        {
            capacity *= 2;
            LinkedList<KeyValuePair<TKey, TValue>>[] newArray = new LinkedList<KeyValuePair<TKey, TValue>>[capacity];
            foreach (var bucket in buckets)
            {
                if (bucket != null)
                {
                    var node = bucket.First;
                    for (int i = 0; i < bucket.Count; i++)
                    {
                        int hashed = GetHash(node.Value.Key);
                        if (newArray[hashed] == null)
                        {
                            newArray[hashed] = new LinkedList<KeyValuePair<TKey, TValue>>();
                            newArray[hashed].AddLast(new KeyValuePair<TKey, TValue>(node.Value.Key, node.Value.Value));
                        }
                    }
                }
            }
            buckets = newArray;
        }
        public bool Remove(TKey key, TValue value)
        {
            if (ContainsKey(key) && ContainsValue(value))
            {
                int temp = GetHash(key);
                var node = new KeyValuePair<TKey, TValue>(key, value);
                if (buckets[temp].Contains(node))
                {
                    buckets[temp].Remove(node);
                    Keys.Remove(key);
                    Values.Remove(value);
                    return true;
                }
            }
            return false;
        }
        public bool ContainsKey(TKey key)
        {
            return Keys.Contains(key);
            //int temp = GetHash(key);
            //if (buckets[temp] == null)
            //{
            //    return false;
            //}
            //foreach (var node in buckets[temp])
            //{
            //    if (node.Key.Equals(key))
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
        public bool ContainsValue(TValue value)
        {

            return Values.Contains(value);
        }
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return Remove(key, GetValue(key));
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (ContainsKey(key) == true)
            {
                int bucketI = GetHash(key);
                if (buckets[bucketI] == null)
                {
                    value = default(TValue);
                    return false;
                }
                else
                {
                    var temp = buckets[bucketI].First;
                    while (temp.Value.Key.Equals(key))
                    {
                        temp = temp.Next;
                    }
                    value = temp.Value.Value;
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
            for (int i = 0; i < capacity; i++)
            {
                if (buckets[i] != null)
                {
                    buckets[i].Clear();
                }
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key) && ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < count)
            {
                throw new Exception("This array aint big nuff for the KeyValuePairs... *The good, the bad, and the ugly plays in the background*");
            }

            //foreach (var bucket in buckets)
            //{
            //    if (bucket != null)
            //    {
            //        foreach (var kvp in bucket)
            //        {
            //            array[arrayIndex] = kvp;
            //            arrayIndex++;
            //        }
            //    }
            //}

            foreach (var key in Keys)
            {
                var val = this[key];
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(key, val);
            }


        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key, item.Value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var key in Keys)
            {
                var val = this[key];
                yield return new KeyValuePair<TKey, TValue>(key, val);
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
