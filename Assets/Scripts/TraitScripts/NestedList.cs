using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NestedList<T>
{
    public List<T> innerList = new List<T>();
}


