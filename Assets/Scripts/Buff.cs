using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public string Name;
    public float Duration;
    public float FlatAmount, PercentAmount;
    public StatType Type;
    public DurationType DurType = DurationType.RealTime;
    public bool Stack = false;
    public enum StatType {AD, AP, Hp, AtkSpeed, Range, Armor, MagicResist, Movespeed, DamageAmp, Durability, Omnivamp, CritChance, CritDamage};
    public enum DurationType {Permanent, RealTime, MatchTime};
    public Buff(string name, float duration, float flatAmount, float percentAmount, StatType type, DurationType durationType, bool stack = false)
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

