using UnityEngine;
using UnityEngine.Events;

public abstract class Attack : MonoBehaviour
{
    public Unit Attacker, Target;
    public Unit.DamageType DamageType;
    public float Hp, MaxHp;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Range;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public float Lifespan;
    public UnityEvent OnAttack, OnDamageDealt;
    // Start is called before the first frame update
    void Start()
    {
        GetStats();
        if(Target)
        {
            OnAttack.Invoke();
            Target.GetComponent<Unit>().TakeDamage(AtkDamage, DamageType, Attacker);
            OnDamageDealt.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Lifespan-=Time.deltaTime;
        if(Lifespan<=0)
            Destroy(gameObject);
    }
    public void GetStats()
    {
        Unit Stats = Attacker.GetComponent<Unit>();
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
    public abstract void AttackLogic();
    public abstract void DamageDealtLogic();
}
