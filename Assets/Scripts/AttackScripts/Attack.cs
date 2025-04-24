using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Attack : MonoBehaviour
{
    public Unit Attacker, Target;
    public enum DamageTypeEnum {AD, AP, True};
    public DamageTypeEnum DamageType;
    public List<Unit.StatType> StatScales = new List<Unit.StatType> {Unit.StatType.AD, Unit.StatType.AP};
    public List<float> ScaleValues = new List<float> {1, 0};
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
    public List<GameObject> LingeringParticles;
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
                DealDamage(Target);
            DamageDealtLogic();
            OnDamageDealt.Invoke();
        }
    }
    public void DealDamage(Unit Target)
    {
        float TotalDamage = 0;
        foreach(Unit.StatType Stat in StatScales)
        {
            TotalDamage += Attacker.StatTypeToStat(Stat) * ScaleValues[StatScales.IndexOf(Stat)];
            //Debug.Log(Stat + " : " + Attacker.StatTypeToStat(Stat) + " * " + ScaleValues[StatScales.IndexOf(Stat)]);
            //Debug.Log("TotalDamage post " + Stat + " : " + TotalDamage);
        }
        if(DamageType == DamageTypeEnum.AD)
        {
            TotalDamage *= 100 / (Target.Armor + 100);
        }
        else if(DamageType == DamageTypeEnum.AP)
        {
            TotalDamage *= 100 / (Target.MagicResist + 100);
        }
        //Debug.Log("TotalDamage post armor and mr: " + TotalDamage);
        TotalDamage *= (1+DamageAmp/100)/(1+Target.Durability/100);
        //Debug.Log("Damage Dealt By " + Attacker.name +  ": " + TotalDamage);
        Target.TakeDamage(TotalDamage, DamageType, Attacker);
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
        foreach(GameObject LingeringParticle in LingeringParticles)
        {
            if(LingeringParticle == null || LingeringParticle.transform.parent == null)
                continue;
            LingeringParticle.transform.parent = null;
            LingeringParticle.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        Destroy(gameObject);
    }
    protected virtual void AttackLogic()
    {

    }
    protected virtual void DamageDealtLogic()
    {
        
    }
}
