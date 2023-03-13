using System;
using System.Collections.Generic;
using UnityEngine;

namespace Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys;
        [SerializeField]
        private List<TValue> _values;

        public Dictionary<TKey, TValue> Dict = new Dictionary<TKey, TValue>();


        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (_keys.Count != _values.Count)
            {
                throw new ArgumentException("Keys and values size mismatch");
            }

            if (new HashSet<TKey>(_keys).Count != _keys.Count)
            {
                throw new ArgumentException("Key values must be unique");
            }
            for (int i = 0; i < _keys.Count; i++)
            {
                Dict[_keys[i]] = _values[i];
            }
        }
    }
}