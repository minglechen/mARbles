using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A serializable dictionary that works in the inspector.
/// </summary>
/// <typeparam name="TKey">The type for the keys.</typeparam>
/// <typeparam name="TValue">The type for the values.</typeparam>
[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [Tooltip("The keys in the dictionary as a list.")] [FormerlySerializedAs("_keys")] [SerializeField]
    private List<TKey> keys;

    [Tooltip("The values in the dictionary as a list.")] [FormerlySerializedAs("_values")] [SerializeField]
    private List<TValue> values;

    /// <summary>
    /// The dictionary to access after deserialization.
    /// </summary>
    public Dictionary<TKey, TValue> Dict = new();

    /// <summary>
    /// Deserializes the lists <see cref="keys"/> and <see cref="values"/> into a dictionary.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if either <see cref="keys"/> has duplicates or
    /// the size of <see cref="keys"/> and <see cref="values"/> are different.</exception>
    public void OnAfterDeserialize()
    {
        if (keys.Count != values.Count)
        {
            throw new ArgumentException("Keys and values size mismatch!");
        }

        if (new HashSet<TKey>(keys).Count != keys.Count)
        {
            throw new ArgumentException("Key values must be unique!");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            Dict[keys[i]] = values[i];
        }
    }

    /// <summary>
    /// Implemented as part of <see cref="ISerializationCallbackReceiver"/>. Not used by <see cref="SerializableDictionary{TKey,TValue}"/>.
    /// </summary>
    public void OnBeforeSerialize()
    {
    }
}