using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Attack : MonoBehaviour
{
    
    public Unit Attacker, Target;
    public Unit.DamageType DamageType;
    public List<Unit.StatType> StatScales = new List<Unit.StatType> {Unit.StatType.AD, Unit.StatType.AP};
    public List<float> ScaleValues = new List<float> {1, 0};
    public float ADScale = 1, APScale;
    public float Hp, MaxHp;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Range;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public float Lifespan = 5;
    [HideInInspector]public UnityEvent OnAttack, OnDamageDealt;
    public GameObject HitEffect;
    public Transform HitEffectPos;
    [Header("Override Attack")]
    public bool SingleOverrideAttack = false;
    protected virtual void Start()
    {
        GetStats();
        if(Lifespan > 0)
            StartCoroutine(DestroyWithLifespan());
        if(SingleOverrideAttack && Attacker)
        {
            Attacker.AttackOverride = null;
        }
    }
    protected IEnumerator DestroyWithLifespan()
    {
        yield return new WaitForSeconds(Lifespan);
        DestroySelf();
    }
    public void GetStats()
    {
        Unit Stats = Attacker;
        Hp = Stats.Hp;
        MaxHp = Stats.MaxHp;
        AtkDamage = Stats.AtkDamage;
        AbilityPower = Stats.AbilityPower;
        AtkSpeed = Stats.AtkSpeed;
        Range = Stats.Range;
        Armor = Stats.Armor;
        MagicResist = Stats.MagicResist;
        Movespeed = Stats.Movespeed;
        DamageAmp = Stats.DamageAmp;
        Durability = Stats.Durability;
        Omnivamp = Stats.Omnivamp;
        CritChance = Stats.CritChance;
        CritDamage = Stats.CritDamage;
    }
    public void TriggerAttack(Unit Target)
    {
        if(Target && Target.gameObject.activeSelf)
        {
            AttackLogic();
            OnAttack.Invoke();
            //float TotalDamage;
            if(!Target.Benched)
                Target.TakeDamage(AtkDamage * ADScale + AbilityPower * APScale, DamageType, Attacker);
            DamageDealtLogic();
            OnDamageDealt.Invoke();
        }
    }
    public void DestroySelf()
    {
        if(HitEffectPos)
        {
            Instantiate(HitEffect, HitEffectPos.position, Quaternion.identity);
        }
        else if(HitEffect)
        {
            Instantiate(HitEffect, transform.position, Quaternion.identity);
        }
        GridGen.Instance.Attacks.Remove(gameObject);
        Destroy(gameObject);
    }
    protected virtual void AttackLogic()
    {

    }
    protected virtual void DamageDealtLogic()
    {
        
    }
}
