using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public bool Component;
    public List<Item> Recipe;
    public int ItemID;
    public Sprite ItemSprite;
    public List<Unit.StatType> StatTypes;
    public List<bool> IsFlatBuff;
    public List<float> StatMagnitudes;
    public abstract void ApplyItem(Unit unit);
    public abstract void UnapplyItem(Unit unit);
}
