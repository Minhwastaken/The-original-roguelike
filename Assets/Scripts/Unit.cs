using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
public class Unit : MonoBehaviour
{
    public int Team;
    public UnitStats Stats;
    public float Hp, MaxHp;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Mana, MaxMana;
    public float Range;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public enum DamageType {AD, AP, True};
    public GameObject Hex, OGHex;
    public bool IsMoving = false, Benched;
    public float LerpProgress = 0;
    [HideInInspector]public Animator Anim;
    public GameObject Target, TargetHex;
    public int Star = 1;
    public (GameObject From, GameObject To) Path;
    private float OGScale;
    public Transform AttackPoint;
    public List<Buff> Buffs = new List<Buff>(), Debuffs = new List<Buff>();
    public UnityEvent OnAbilityUse = new UnityEvent();
    private void Start() 
    {
        GameManager.Instance.OnBattleStart.AddListener(() => StartCoroutine(Action()));
        if(Team == 1)
            GridGen.Instance.Enemies.Add(gameObject);
        GetStats();
        Anim = GetComponent<Animator>();
        OGScale = transform.localScale.x;
        OnStart();
    }
    private void Update()
    {
        if(Buffs.Count > 0)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                Buffs[i].Duration -= Time.deltaTime;
                if (Buffs[i].Duration <= 0 && Buffs[i].Duration != -1)
                {
                    UpdateTrueStats(Buffs[i].Type);
                    Buffs.RemoveAt(i);
                }
            }
        }
    }
    public IEnumerator Action()
    {
        if(!Benched)
        {
            while(GameManager.Instance.State == GameManager.GameState.Battle)
            {
                if(!AttackableTarget())
                {   
                    //Debug.Log("No target");
                    if(GetTargetInRange())
                    {
                        //Debug.Log("Target in range available");
                        Target = GetTargetInRange();
                    }
                    else if(GetTarget() && Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    {
                        if(!IsMoving)
                        {
                            TargetHex = Pathfind(Hex, GetTarget().GetComponent<Unit>().Hex)[0];
                            IsMoving = true;
                            Path = (Hex, TargetHex);
                            TargetHex.GetComponent<Hex>().Taken = gameObject;
                        }   
                        //Debug.Log("Moving to next target");
                        else
                            Move();
                    }
                }
                else if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    if(Stats.Ability && Mana >= MaxMana)
                    {
                        UseAbility();
                    }
                    //Debug.Log("Target attackable");
                    else
                        Attack();
                }
                yield return null;
            }
        }
    }
    protected virtual void Attack()
    {
        if(Target)
        {
            transform.up = (Vector2) (Target.transform.position - transform.position);
            GainMana(10);
            Anim.Play("Attack");
        }
    }
    protected virtual void UseAbility()
    {
        if(Target)
        {
            transform.up = (Vector2) (Target.transform.position - transform.position);
            Mana -= MaxMana;
            OnAbilityUse.Invoke();
            Anim.Play("Ability");
        }
    }
    public void SpawnAttack(GameObject AttackGO)
    {
        GameObject Attack = Instantiate(AttackGO, transform.position, transform.rotation);
        GridGen.Instance.Attacks.Add(Attack);
        Attack.transform.localScale = new Vector3(Attack.transform.localScale.x * Mathf.Pow(Stats.SizeScale, Star - 1), Attack.transform.localScale.y * Mathf.Pow(Stats.SizeScale, Star - 1), 1);
        Attack.GetComponent<Attack>().Attacker = this;
        if(Target)
        {
            Attack.GetComponent<Attack>().Target = Target.GetComponent<Unit>();
            if(Attack.GetComponent<Projectile>())
            {
                SetUpProjectile(Attack, Target);
            }
            if(Attack.GetComponent<AOEAttack>())
            {
                if(Attack.GetComponent<AOEAttack>().SpawnAtTarget)
                {
                    Attack.transform.position = Target.transform.position;
                }
            }
        }
        else
        {
            if(Attack.GetComponent<Projectile>())
            {
                SetUpProjectile(Attack, GetTarget());
            }
            if(Attack.GetComponent<AOEAttack>())
            {
                if(Attack.GetComponent<AOEAttack>().SpawnAtTarget)
                {
                    Attack.transform.position = GetTarget().transform.position;
                }
            }
        }
    }
    protected void GainMana(int amount)
    {
        Mana += amount;
    }
    private void SetUpProjectile(GameObject Attack, GameObject Target)
    {
        Attack.GetComponent<Projectile>().Direction =((Vector2) (Target.transform.position - transform.position)).normalized;
        Attack.transform.position = AttackPoint.position;
        Attack.transform.rotation = Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.up, Attack.GetComponent<Projectile>().Direction));
    }
    private void Move()
    {
       // Debug.Log(Hex + " " + EndHex);
        LerpProgress += Time.deltaTime * Movespeed / 100f;
        transform.position = Vector2.Lerp(Path.From.transform.position, Path.To.transform.position, LerpProgress);
        if(LerpProgress >= 0.1f && Hex.GetComponent<Hex>().Taken == gameObject)
        {
            Hex.GetComponent<Hex>().Taken = null;
        }
        if(LerpProgress > 0.8f && Hex!=Path.To)
        {
            Hex = Path.To;
        }
        if(LerpProgress > 0.95f)
        {
            LerpProgress = 0;
            transform.position = Path.To.transform.position + new Vector3(0,0,-1);
            IsMoving = false;
            Path = (null, null);
        }
    }
    public void GetStats()
    {
        Hp = Stats.Hp;
        MaxHp = Stats.MaxHp;
        Mana = Stats.Mana;
        MaxMana = Stats.MaxMana;
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
    private List<GameObject> Pathfind(GameObject StartHex, GameObject EndHex)
    {
        if(HexDist(StartHex, EndHex) <= Range)
            return new List<GameObject>();

        Queue<GameObject> BFSQueue = new Queue<GameObject>();
        Dictionary<GameObject, GameObject> CameFrom = new Dictionary<GameObject, GameObject>();
        BFSQueue.Enqueue(StartHex);

        while(BFSQueue.Count > 0)
        {
            GameObject CurHex = BFSQueue.Dequeue();
            foreach(GameObject AdjHex in CurHex.GetComponent<Hex>().AdjHexes)
            {

                Hex AdjHexHex = AdjHex.GetComponent<Hex>();
                
                bool CheckTaken = false;
                if (AdjHexHex.Taken == null || AdjHexHex.Taken == gameObject)
                {
                    CheckTaken = true;
                }
                else if (AdjHexHex.Taken)
                {
                    Unit takenUnit = AdjHexHex.Taken.GetComponent<Unit>();
                    if (takenUnit != null)
                    {
                        if (takenUnit.Team != Team)
                        {
                            CheckTaken = true;
                        }
                    }
                }


                foreach(GameObject Unit in GridGen.Instance.Units)
                {
                    if(Unit)
                        if(Unit.GetComponent<Unit>().Path.To == AdjHex && Unit.GetComponent<Unit>().Team == Team && Unit != gameObject)
                            CheckTaken = false;
                }


                if(!CameFrom.ContainsKey(AdjHex) && CheckTaken) //Also check if it's occupied in the future.
                {
                    BFSQueue.Enqueue(AdjHex);
                    CameFrom.Add(AdjHex, CurHex);
                    if(HexDist(AdjHex, EndHex) <= Range)
                    {
                        return BuildPath(CameFrom, StartHex, AdjHex);
                    }
                }
            }
        }
        return new List<GameObject>();
    }

    private List<GameObject> BuildPath(Dictionary<GameObject,GameObject> CameFrom ,GameObject StartHex, GameObject EndHex)
    {
        List<GameObject> Path = new List<GameObject>();

        GameObject CurHex = EndHex;
        while(CurHex != StartHex)
        {
            Path.Add(CurHex);
            CurHex = CameFrom[CurHex];
        }
        Path.Reverse();

        //Debug.Log("Build Succeeded");
        return Path;
    }
    public GameObject GetTarget()
    {
        int MinCnt = 99;
        GameObject ClosestEnemy = null;
        List<GameObject> Targets= new List<GameObject>();
        if(Team == 0)
        {
            Targets = GridGen.Instance.Enemies;    
        }
        else
        {
            Targets = GridGen.Instance.Units;
        }
        foreach(GameObject Target in Targets)
        {
            if(!ValidTarget(Target))
                continue;
            int PathLen = Pathfind(Hex, Target.GetComponent<Unit>().Hex).Count;
            if(PathLen != 0 && PathLen < MinCnt)
            {
                ClosestEnemy = Target;
                MinCnt = PathLen;
            }
        }
        return ClosestEnemy;
    }
    private GameObject GetTargetInRange()
    {
        float MinDist = 99;
        GameObject ClosestEnemy = null;
        List<GameObject> Targets= new List<GameObject>();
        if(Team == 0)
        {
            Targets = GridGen.Instance.Enemies;    
        }
        else
        {
            Targets = GridGen.Instance.Units;
        }

        if(Targets.Count == 0)
            return null;

        foreach(GameObject Target in Targets)
        {
            if(!ValidTarget(Target))
                continue;
            float Dist = Vector2.Distance(transform.position, Target.transform.position);
            if(Dist < MinDist && Dist < Range)
            {
                ClosestEnemy = Target;
                MinDist = Dist;
            }
        }

        int MinHexDist = 99;
        if(ClosestEnemy == null)
        {
            foreach(GameObject Target in Targets)
            {
                if(Target == null || Target.GetComponent<Unit>().Hex == null)
                    continue;
                if(HexDist(Hex, Target.GetComponent<Unit>().Hex) <= Range && HexDist(Hex, Target.GetComponent<Unit>().Hex) < MinHexDist)
                {
                    ClosestEnemy = Target;
                    MinHexDist = HexDist(Hex, Target.GetComponent<Unit>().Hex);
                }
            }
        }
        return ClosestEnemy;
    }
    private bool AttackableTarget()
    {
        if(ValidTarget(Target))
            if(Vector2.Distance(transform.position, Target.transform.position) < Range || HexDist(Hex, Target.GetComponent<Unit>().Hex) <= Range)
            {
                return true;
            }
        return false;
    }
    
    private int HexDist(GameObject Hex, GameObject TargetHex)
    {
        int X1 = Hex.GetComponent<Hex>().X, Y1 = Hex.GetComponent<Hex>().Y, X2 = TargetHex.GetComponent<Hex>().X, Y2 = TargetHex.GetComponent<Hex>().Y;
        int left = 0, right = 0;
        if(Y1 % 2 != Y2 % 2)
        {
            if(Y1 % 2 == 0)
            {
                left = 0;
                right = 1;
            }
            else
            {
                left = -1;
                right = 0;
            }

        }
        int FloorRange = (int)Mathf.Floor(Range);
        int YOffset = FloorRange - Mathf.Abs(Y1 - Y2);
        int MinX = -YOffset + left, MaxX = YOffset + right;

        if(Y2 >= Y1 - FloorRange && Y2 <= Y1 + FloorRange && X2 >= X1 + MinX && X2 <= X1 + MaxX)
        {
            return Mathf.Min(Mathf.Abs(X1 + MinX - X2), X1 + MaxX - X2);
        }
        else
        {
            return 99;
        }
    }
    
    public void TakeDamage(float damage, DamageType type, Unit Attacker)
    {
        Hp -= damage;
        if(Hp <= 0 && gameObject.activeSelf)
        {
            if(Team == 0)
            {
                GridGen.Instance.Units.Remove(gameObject);
                GridGen.Instance.DeadUnits.Add(gameObject);
                Hex.GetComponent<Hex>().Taken = null;
                Path = (null, null);
                gameObject.SetActive(false);
                if(GridGen.Instance.Units.Count == 0)
                {
                    GameManager.Instance.ChangeState();
                }
            }
            else
            {
                GridGen.Instance.Enemies.Remove(gameObject);
                Hex.GetComponent<Hex>().Taken = null;
                Destroy(gameObject);
                if(GridGen.Instance.Enemies.Count == 0)
                {
                    GameManager.Instance.ChangeState();
                }
            }
            Attacker.gameObject.transform.rotation = Quaternion.identity;
        }
    }
    private bool ValidTarget(GameObject Target)
    {
        if(Target == null || !Target.activeSelf)
        {
            return false;
        }
        return true;
    }

    protected virtual void OnStart()
    {
    }

    [ContextMenu("Update Stats")]
    public void UpdateBaseStats()
    {
        MaxHp = Stats.MaxHp * Mathf.Pow(Stats.HpScale, Star - 1);
        AtkDamage = Stats.AtkDamage * Mathf.Pow(Stats.ADScale, Star - 1);
        float Scale = Mathf.Pow(Stats.SizeScale, Star - 1) * OGScale;
        transform.localScale = new Vector3(Scale, Scale, 1);
    }
    public void UpdateTrueStats(Buff.BuffType Type)
    {
        float FlatAmount = 0, PercentAmount = 0;
        ref float Stat = ref BuffTypeToStat(Type);
        float BaseStat = BuffTypeToBaseStat(Type);

        foreach(Buff buff in Buffs)
        {
            if(buff.Type == Type)
            {
                FlatAmount += buff.FlatAmount;
                PercentAmount += buff.PercentAmount;
            }
        }

        Stat = BaseStat + FlatAmount;
        Stat *= 1 + PercentAmount;
    }
    public void GainBuff(Buff buff)
    {
        for (int i = 0; i < Buffs.Count; i++)
        {   
            if (Buffs[i].Name == buff.Name)
            {
                if (buff.Stack)
                {
                    Buffs[i].FlatAmount += buff.FlatAmount;
                    Buffs[i].PercentAmount += buff.PercentAmount;
                    return;
                }
                else
                {   
                    Buffs[i] = buff;
                    return;
                }
            }
        }

        Buffs.Add(buff);
        Debug.Log("Buffed " + buff.Type + " by " + buff.FlatAmount + " " + buff.PercentAmount);
        UpdateTrueStats(buff.Type);
    }
    public void ClearBuffs()
    {
        HashSet<Buff.BuffType> CurrentlyBuffedStats = new HashSet<Buff.BuffType>();
        foreach(Buff buff in Buffs)
        {
            CurrentlyBuffedStats.Add(buff.Type);
        }
        Buffs.Clear();
        foreach(Buff.BuffType Type in CurrentlyBuffedStats)
        {
            UpdateTrueStats(Type);
        }
    }
    private ref float BuffTypeToStat(Buff.BuffType Type)
    {
        if(Type == Buff.BuffType.Hp)
        {
            return ref Hp;
        }
        else if(Type == Buff.BuffType.AD)
        {
            return ref AtkDamage;
        }
        else if(Type == Buff.BuffType.AP)
        {
            return ref AbilityPower;
        }
        else if(Type == Buff.BuffType.AtkSpeed)
        {
            return ref AtkSpeed;
        }
        else if(Type == Buff.BuffType.Armor)
        {
            return ref Armor;
        }
        else if(Type == Buff.BuffType.MagicResist)
        {
            return ref MagicResist;
        }
        else if(Type == Buff.BuffType.Movespeed)
        {
            return ref Movespeed;
        }
        else if(Type == Buff.BuffType.DamageAmp)
        {
            return ref DamageAmp;
        }
        else if(Type == Buff.BuffType.Durability)
        {
            return ref Durability;
        }
        else if(Type == Buff.BuffType.Omnivamp)
        {
            return ref Omnivamp;
        }
        else if(Type == Buff.BuffType.CritChance)
        {
            return ref CritChance;
        }
        else if(Type == Buff.BuffType.CritDamage)
        {
            return ref CritDamage;
        }
        else
        {
            Debug.Log(Type);
            return ref AtkDamage;
        }
    }
    private float BuffTypeToBaseStat(Buff.BuffType Type)
    {
        if(Type == Buff.BuffType.Hp)
        {
            return Stats.MaxHp * Mathf.Pow(Stats.HpScale, Star - 1);
        }
        else if(Type == Buff.BuffType.AD)
        {
            return Stats.AtkDamage * Mathf.Pow(Stats.ADScale, Star - 1);
        }
        else if(Type == Buff.BuffType.AP)
        {
            return Stats.AbilityPower;
        }
        else if(Type == Buff.BuffType.AtkSpeed)
        {
            return Stats.AtkSpeed;
        }
        else if(Type == Buff.BuffType.Armor)
        {
            return Stats.Armor;
        }
        else if(Type == Buff.BuffType.MagicResist)
        {
            return Stats.MagicResist;
        }
        else if(Type == Buff.BuffType.Movespeed)
        {
            return Stats.Movespeed;
        }
        else if(Type == Buff.BuffType.DamageAmp)
        {
            return Stats.DamageAmp;
        }
        else if(Type == Buff.BuffType.Durability)
        {
            return Stats.Durability;
        }
        else if(Type == Buff.BuffType.Omnivamp)
        {
            return Stats.Omnivamp;
        }
        else if(Type == Buff.BuffType.CritChance)
        {
            return Stats.CritChance;
        }
        else if(Type == Buff.BuffType.CritDamage)
        {
            return Stats.CritDamage;
        }
        else
        {
            Debug.Log(Type);
            return Stats.AtkDamage;
        }
    }
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        for(int i = 0; i<360; i++)
        {
            //Gizmos.DrawLine(transform.position + new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad), 0) * Stats.Range, transform.position + new Vector3(Mathf.Sin((i+1) * Mathf.Deg2Rad), Mathf.Cos((i+1) * Mathf.Deg2Rad), 0) * Stats.Range);
        }
        Gizmos.color = Color.blue;
        if(Path.From && Path.To)
            Gizmos.DrawLine(Path.From.transform.position, Path.To.transform.position);
    }
}
