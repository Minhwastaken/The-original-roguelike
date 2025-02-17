using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public string Name;
    public float Duration;
    public float FlatAmount, PercentAmount;
    public BuffType Type;
    public bool Stack = false;
    public enum BuffType {AD, AP, Hp, AtkSpeed, Range, Armor, MagicResist, Movespeed, DamageAmp, Durability, Omnivamp, CritChance, CritDamage};
    public Buff(string name, float duration, float flatAmount, float percentAmount, BuffType type, bool stack = false)
    {
        Name = name;
        Duration = duration;
        FlatAmount = flatAmount;
        PercentAmount = percentAmount;
        Type = type;
        Stack = stack;
    }
}

