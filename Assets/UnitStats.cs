using UnityEngine;
[CreateAssetMenu(menuName = "Unit")]
public class UnitStats : ScriptableObject
{
    [Header("Basics")]
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
    [Header("Abilities")]
    public GameObject Attack;
    public GameObject Ability;
    [Header("Visuals")]
    public Sprite ShopIcon;
}
