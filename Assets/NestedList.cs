using System.Collections.Generic;

[System.Serializable]
public class NestedList<T>
{
    public List<T> innerList = new List<T>();
}
