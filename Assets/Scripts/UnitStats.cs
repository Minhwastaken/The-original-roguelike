using UnityEngine;
[CreateAssetMenu(menuName = "Unit")]
public class UnitStats : ScriptableObject
{
    [Header("Basics")]
    public int Cost = 1;
    public float Hp;
    public float MaxHp;
    public float Range;
    public float MaxMana;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public float ADScale = 1.8f, HpScale = 1.8f;
    public float SizeScale = 1.1f;
    [Header("Abilities")]
    public GameObject Attack;
    public GameObject Ability;
    [Header("Visuals")]
    public GameObject Unit;
    public Sprite ShopIcon;
    public enum Role {AttackCarry, MagicCarry, AttackCaster, MagicCaster, AttackTank, MagicTank, AttackFighter, MagicFighter}
}
