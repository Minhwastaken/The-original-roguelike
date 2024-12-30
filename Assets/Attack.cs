using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject Attacker, Target;
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
    // Start is called before the first frame update
    void Start()
    {
        GetStats();
        if(Target)
        {
            Target.GetComponent<Unit>().TakeDamage(AtkDamage, DamageType);
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
        UnitStats Stats = Attacker.GetComponent<Unit>().Stats;
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
}
