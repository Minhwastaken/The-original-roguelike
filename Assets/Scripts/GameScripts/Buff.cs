using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public string Name;
    public float Duration;
    public float FlatAmount, PercentAmount;
    public Unit.StatType Type;
    public DurationType DurType = DurationType.RealTime;
    public bool Stack = false;
    public enum DurationType {Permanent, RealTime, MatchTime};
    public Buff(string name, float duration, float flatAmount, float percentAmount, Unit.StatType type, DurationType durationType, bool stack = false)
    {
        Name = name;
        Duration = duration;
        FlatAmount = flatAmount;
        PercentAmount = percentAmount;
        Type = type;
        DurType = durationType;
        Stack = stack;
    }
}

