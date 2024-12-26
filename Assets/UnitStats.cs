using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit")]
public class UnitStats : ScriptableObject
{
    public float Hp, MaxHp;
    public float Range;
    public float MaxMana;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
}
