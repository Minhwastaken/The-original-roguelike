using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NestedList<T>
{
    public List<T> innerList = new List<T>();
}
[Serializable]
public class KeyValuePair<K, V> 
{
    public K StatType;
    public V Scale;
    
    public KeyValuePair(K StatType, V Scale) 
    {
        this.StatType = StatType;
        this.Scale = Scale;
    }
}

[Serializable]
public class SerializableDictionary<K, V> 
{
    [SerializeField] private List<KeyValuePair<K, V>> entries = new();

    public void FromDictionary(Dictionary<K, V> dict) 
    {
        entries.Clear();
        foreach (var kvp in dict) {
            entries.Add(new KeyValuePair<K, V>(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<K, V> ToDictionary() 
    {
        Dictionary<K, V> dict = new();
        foreach (var entry in entries) {
            dict[entry.StatType] = entry.Scale;
        }
        return dict;
    }
}

