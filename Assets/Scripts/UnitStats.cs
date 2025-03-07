using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Unit")]
public class UnitStats : ScriptableObject
{
    [Header("Basics")]
    public int Cost = 1;
    public enum UnitRole {AttackCarry, MagicCarry, AttackCaster, MagicCaster, AttackTank, MagicTank, AttackFighter, MagicFighter, AttackReaper, MagicReaper};
    public UnitRole Role; 
    public List<TraitDefinition.Traits> Traits = new List<TraitDefinition.Traits>();
    public float Hp;
    public float MaxHp;
    public float Range;
    public float Mana, MaxMana;
    public float AtkDamage, AbilityPower = 1;
    public float AtkSpeed;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance = 25, CritDamage = 1.4f;
    public float ADScale = 1.8f, HpScale = 1.8f;
    public float SizeScale = 1.1f;
    [Header("Abilities")]
    public GameObject Attack;
    public GameObject Ability;
    [Header("Visuals")]
    public GameObject Unit;
    public Sprite ShopIcon;
    [Header("Specials")]
    public bool AbilityAffectedByAtkSpeed = false;
}
