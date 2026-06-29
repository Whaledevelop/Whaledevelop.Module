using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Whaledevelop.Utility
{
    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Entry
        {
            [field: SerializeField]
            public TKey Key { get; set; }

            [field: SerializeField]
            public TValue Value { get; set; }
        }
        
        [HideInInspector]
        [SerializeField]
        private Entry[] _entries = Array.Empty<Entry>();
        
        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();

            foreach (var entry in _entries)
            {
                this[entry.Key] = entry.Value;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            var entries = new List<Entry>();
            foreach (var (key, value) in this)
            {
                var item = new Entry
                {
                    Key = key,
                    Value = value
                };
                entries.Add(item);
            }

#if UNITY_EDITOR
            if (typeof(TKey).IsEnum)
            {
                entries.Sort((x, y) => Comparer<int>.Default.Compare((int)(object)x.Key, (int)(object)y.Key));
            }
            else if (typeof(IComparable<TKey>).IsAssignableFrom(typeof(TKey)))
            {
                entries.Sort((x, y) => Comparer<TKey>.Default.Compare(x.Key, y.Key));
            }
#endif

            _entries = entries.ToArray();
        }

        #endregion
        
        public SerializableDictionary()
        {
            
        }
        
        #if UNITY_EDITOR
        
        public SerializableDictionary(Dictionary<TKey, TValue> dictionary)
        {
            SetFromDictionary(dictionary);
        }
        
        public void SetFromDictionary(Dictionary<TKey, TValue> dictionary)
        {
            Clear();

            foreach (var kvp in dictionary)
            {
                this[kvp.Key] = kvp.Value;
            }

            (this as ISerializationCallbackReceiver).OnBeforeSerialize();
        }
        
        #endif

    }
}