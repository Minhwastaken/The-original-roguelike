using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit")]
public class UnitStats : ScriptableObject
{
    [Header("Basics")]
    public int Cost = 1;
    public float Hp;
    public float MaxHp;
    public float Range;
    public float Mana, MaxMana = 50;
    public float AtkDamage, AbilityPower = 1;
    public float AtkSpeed;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance = 25, CritDamage = 1.4f;
    public float ADScale = 1.8f, HpScale = 1.8f;
    public float SizeScale = 1.1f;
}
