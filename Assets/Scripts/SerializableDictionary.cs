using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.Serialization;
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
                throw new DataException("Keys and values size mismatch");
            }

            if (new HashSet<TKey>(_keys).Count != _keys.Count)
            {
                throw new ConstraintException("Key values must be unique");
            }
            for (int i = 0; i < _keys.Count; i++)
            {
                Dict.Add(_keys[i], _values[i]);
            }
        }
    }
}