using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class Unit : MonoBehaviour, IDamageable
{
    public int Team;
    public UnitStats Stats;
    public enum UnitRole {AttackCarry, MagicCarry, AttackCaster, MagicCaster, AttackTank, MagicTank, AttackFighter, MagicFighter, AttackReaper, MagicReaper};
    public UnitRole Role; 
    public List<TraitDefinition.Traits> Traits = new List<TraitDefinition.Traits>();
    public enum StatType {AD, AP, MaxHp, Hp, AtkSpeed, Range, Armor, MagicResist, Movespeed, DamageAmp, Durability, Omnivamp, CritChance, CritDamage, Mana};
    public float Hp, MaxHp, Shield = 0;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Mana, MaxMana;
    public float Range;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public List<Item> Items;
    [HideInInspector] public float FlatArmorPierce = 0, PercentArmorPierce = 0, FlatMRPierce = 0, PercentMRPierce = 0;
    public GameObject Hex, OGHex;
    public bool IsMoving = false, Benched;
    [HideInInspector] public float LerpProgress = 0;
    [HideInInspector]public Animator Anim;
    [HideInInspector]public int AttackCount = 0, AbilityCount = 0;
    public GameObject Target, TargetHex;
    public int Star = 1;
    public (GameObject From, GameObject To) Path;
    [HideInInspector]public float OGScale = 1, HealAmount = 0, ManaAmount = 0, AttackScale = 1;
    [HideInInspector]public GameObject SpawnedAttack;
    public Transform AttackPoint;
    public GameObject Sprites;
    public List<Buff> Buffs = new List<Buff>(), Debuffs = new List<Buff>();
    [HideInInspector]public UnityEvent OnAttack = new UnityEvent(), OnAbilityUse = new UnityEvent(), OnKill = new UnityEvent(),
    OnDeath = new UnityEvent(), OnHeal = new UnityEvent(), OnTakeDamage = new UnityEvent(),
    OnReset = new UnityEvent(), OnManaGain = new UnityEvent(), OnStarUp = new UnityEvent(),
    OnBuffGain = new UnityEvent();
    [Header("Abilities")]
    public GameObject AttackPrefab;
    public GameObject AbilityPrefab;
    public GameObject AttackOverride;
    [Header("Visuals")]
    public Sprite ShopIcon;
    public bool TurnOnMove = true, TurnOnAttack = true, HasManaBar = true;
    [Header("Specials")]
    public bool PassiveAbility = false;
    public bool AbilityAffectedByAtkSpeed = false;
    protected virtual void Start() 
    {
        Anim = GetComponent<Animator>();
        OGScale = transform.localScale.x;
        GameManager.Instance.OnBattleStart.AddListener(() => StartCoroutine(Action()));
        Sprites = transform.Find("Sprite").gameObject;
        GetStats();
    }
    public IEnumerator Action()
    {
        if(!Benched)
        {
            while(GameManager.Instance.State == GameManager.GameState.Battle)
            {
                UpdateRealTimeBuffs();
                if(!Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    yield return null;
                    continue;
                }
                if(!AttackableTarget())
                {   
                    if(GetTargetInRange())
                    {
                        Target = GetTargetInRange();
                    }
                    else if(GetTarget())
                    {
                        if(!IsMoving)
                        {
                            TargetHex = Pathfind(Hex, GetTarget().GetComponent<Unit>().Hex)[0];
                            IsMoving = true;
                            Path = (Hex, TargetHex);
                            TargetHex.GetComponent<Hex>().Taken = gameObject;
                        }   
                        else
                            Move();
                    }
                }
                else
                {
                    if(AbilityPrefab && Mana >= MaxMana)
                    {
                        UseAbility();
                    }
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
            AttackCount++;
            if(TurnOnAttack)
            {
                Sprites.transform.up = (Vector2) (Target.transform.position - transform.position);
            }
            OnAttack.Invoke();
            GainMana(10);
            if(AttackOverride && Anim.HasState(0, Animator.StringToHash("AttackOverride")))
            {
                Anim.Play("AttackOverride");
            }
            else
            {
                Anim.Play("Attack");
            }
        }
    }
    protected virtual void UseAbility()
    {
        if(Target)
        {
            AbilityCount++;
            if(TurnOnAttack)
                Sprites.transform.up = (Vector2) (Target.transform.position - transform.position);
            Mana -= MaxMana;
            OnAbilityUse.Invoke();
            if(!PassiveAbility)
                Anim.Play("Ability");
        }
    }
    public void SpawnAttack(GameObject AttackGO)
    {
        if(Sprites == null)
            return;
        SpawnedAttack = Instantiate(AttackGO, Sprites.transform.position, Sprites.transform.rotation);
        SpawnedAttack.GetComponent<Attack>().Attacker = this;
        GridGen.Instance.Attacks.Add(SpawnedAttack);

        float TrueAttackScale = SpawnedAttack.transform.localScale.x * Mathf.Pow(Stats.SizeScale, Star - 1) * AttackScale;
        SpawnedAttack.transform.localScale = new Vector3(TrueAttackScale, TrueAttackScale, 1);
        foreach(Transform child in SpawnedAttack.transform)
        {
            if(child.GetComponent<ParticleSystem>())
            {
                child.localScale = new Vector3(TrueAttackScale, TrueAttackScale, TrueAttackScale);
            }
        }
        if(AttackPoint)
        {
            SpawnedAttack.transform.position = AttackPoint.position;
        }
        if(Target)
        {
            SetUpAttack(SpawnedAttack, Target);
        }
        else
        {
            SetUpAttack(SpawnedAttack, GetTarget());
        }
    }
    private void SetUpAttack(GameObject Attack, GameObject Target)
    {
        if(Target == null)
        {
            return;
        }
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
            else
            {
                Attack.transform.position = transform.position;
            }
        }
    }
    public void GainMana(float amount, bool IsBonus = false)
    {
        Mana += amount;
        if(!IsBonus)
        {
            ManaAmount = amount;
            OnManaGain.Invoke();
        }
    }
    private void SetUpProjectile(GameObject Attack, GameObject Target)
    {
        Attack.GetComponent<Projectile>().Direction =((Vector2) (Target.transform.position - Attack.transform.position)).normalized;
        Attack.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Attack.GetComponent<Projectile>().Direction));
    }
    private void Move()
    {
       // Debug.Log(Hex + " " + EndHex);
        if(TurnOnMove)
        {
            Sprites.transform.up = (Vector2) (Path.To.transform.position - Path.From.transform.position);
        }
        LerpProgress += Time.deltaTime * Movespeed / 100f;
        transform.position = Vector2.Lerp(Path.From.transform.position, Path.To.transform.position, LerpProgress);
        if(LerpProgress >= 0.3f && Hex.GetComponent<Hex>().Taken == gameObject)
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
    public List<GameObject> Pathfind(GameObject StartHex, GameObject EndHex)
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
                foreach(GameObject Unit in GridGen.Instance.Enemies)
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
        List<GameObject> Targets;
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
    public GameObject GetTargetInRange()
    {
        float MinDist = 99;
        GameObject ClosestEnemy = null;
        List<GameObject> Targets;
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
                int HexDistToTarget = HexDist(Hex, Target.GetComponent<Unit>().Hex);
                if(HexDistToTarget < MinHexDist && HexDistToTarget <= Range)
                {
                    ClosestEnemy = Target;
                    MinHexDist = HexDistToTarget;
                }
            }
        }
        return ClosestEnemy;
    }
    private bool AttackableTarget()
    {
        if(ValidTarget(Target))
        {
            int HexDistToTarget = 99;
            if (Target.GetComponent<Unit>().Hex)
            {
                HexDistToTarget = HexDist(Hex, Target.GetComponent<Unit>().Hex);
            }
            if(Vector2.Distance(transform.position, Target.transform.position) < Range || HexDistToTarget <= Range)
            {
                if(Target.GetComponent<Unit>().Hp > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    //Check how far the target is from the unit in hexes. The lower the number, the farther it is. Must be >0 to be within range.
    /*public int HexDist(GameObject Hex, GameObject TargetHex)
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
        return Mathf.Min(Mathf.Abs(X1 + MinX - X2), X1 + MaxX - X2);
    }*/
    public int HexDist(GameObject Hex, GameObject TargetHex)
    {
        int X1 = Hex.GetComponent<Hex>().X, Y1 = Hex.GetComponent<Hex>().Y, X2 = TargetHex.GetComponent<Hex>().X, Y2 = TargetHex.GetComponent<Hex>().Y;
        int offset = 0;
        int YOffset = Mathf.Abs(Y1 - Y2);
        if(Mathf.Abs(X1 - X2) > YOffset/2)
        {
            offset += Mathf.Abs(X1 - X2) - YOffset/2;
        }
        return offset + YOffset;
    }
    public void TakeDamage(float Damage, Attack.DamageTypeEnum DamageType, Unit Attacker)
    {
        if(Shield < Damage)
        {
            Hp -= Damage - Shield;
            if(Hp <= 0)
            {
                //float OverkillDamage = -Hp;
                Hp = 0;
            }
        }
        else
        {
            Shield -= Damage;
        }
        OnTakeDamage.Invoke();
        if(Hp == 0)
        {
            if(gameObject.activeSelf)
            {
                Die(Attacker);
            }
        }
    }
    public void Die(Unit Attacker)
    {
        Attacker.OnKill.Invoke();
        OnDeath.Invoke();
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
    }
    public void RemoveUnit()
    {
        if(Team == 0)
        {
            if(Benched)
            {
                GridGen.Instance.BenchedUnits.Remove(gameObject);
                Hex.GetComponent<BenchSlot>().Taken = null;
            }
            else
            {
                GridGen.Instance.Units.Remove(gameObject);
                GameManager.Instance.RemoveTraits(this);
                Hex.GetComponent<Hex>().Taken = null;
            }
        }
        else
        {
            GridGen.Instance.Enemies.Remove(gameObject);
            Hex.GetComponent<Hex>().Taken = null;
        }
        Destroy(gameObject);
    }
    public void Heal(Unit Target, float amount)
    {
        if(Target.Hp + amount > Target.MaxHp)
        {
            amount = Target.MaxHp - Target.Hp;
        }
        Target.Hp += amount;
        Target.HealAmount = amount;
        Target.OnHeal.Invoke();
    }
    private bool ValidTarget(GameObject Target)
    {
        if(Target == null || !Target.activeSelf)
        {
            return false;
        }
        return true;
    }

    [ContextMenu("Update Stats")]
    public void UpdateBaseStats()
    {
        MaxHp = Stats.MaxHp * Mathf.Pow(Stats.HpScale, Star - 1);
        AtkDamage = Stats.AtkDamage * Mathf.Pow(Stats.ADScale, Star - 1);
        float Scale = Mathf.Pow(Stats.SizeScale, Star - 1) * OGScale;
        transform.localScale = new Vector3(Scale, Scale, 1);
    }
    public void UpdateTrueStats(StatType Type)
    {
        float FlatAmount = 0, PercentAmount = 0;
        ref float Stat = ref StatTypeToStat(Type);
        float BaseStat = StatTypeToBaseStat(Type);

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
        Buff BuffCopy = new Buff(buff.Name, buff.Duration, buff.FlatAmount, buff.PercentAmount, buff.Type, buff.DurType, buff.Stack);
        for (int i = 0; i < Buffs.Count; i++)
        {
            if (Buffs[i].Name == buff.Name)
            {
                if (buff.Stack)
                {
                    Buffs[i].FlatAmount += buff.FlatAmount;
                    Buffs[i].PercentAmount += buff.PercentAmount;
                    UpdateTrueStats(buff.Type);

                    if (BuffCopy.Type == StatType.MaxHp)
                    {
                        Heal(this, BuffCopy.FlatAmount + MaxHp * BuffCopy.PercentAmount / (1 + BuffCopy.PercentAmount));
                    }
                    OnBuffGain.Invoke();
                    return;
                }
                else
                {
                    Buffs[i] = BuffCopy;
                    UpdateTrueStats(buff.Type);

                    if (BuffCopy.Type == StatType.MaxHp)
                    {
                        Heal(this, BuffCopy.FlatAmount + MaxHp * BuffCopy.PercentAmount / (1 + BuffCopy.PercentAmount));
                    }
                    OnBuffGain.Invoke();
                    return;
                }
            }
        }

        Buffs.Add(BuffCopy);
        //Debug.Log("Buffed " + buff.Type + " by " + buff.FlatAmount + " " + buff.PercentAmount);
        UpdateTrueStats(buff.Type);

        if (BuffCopy.Type == StatType.MaxHp)
        {
            Heal(this, BuffCopy.FlatAmount + MaxHp * BuffCopy.PercentAmount / (1 + BuffCopy.PercentAmount));
        }
        OnBuffGain.Invoke();
    }
    public void RemoveBuff(Buff buff)
    {
        StatType BuffType = buff.Type;
        foreach(Buff AppliedBuff in Buffs)
        {
            if(AppliedBuff.Name == buff.Name)
            {
                Buffs.Remove(AppliedBuff);
                break;
            }
        }
        UpdateTrueStats(BuffType);
    }
    public void ClearBuffs()
    {
        HashSet<StatType> CurrentlyBuffedStats = new HashSet<StatType>();
        foreach (Buff buff in Buffs)
        {
            CurrentlyBuffedStats.Add(buff.Type);
            Debug.Log(buff.Name + " " + buff.Type.ToString());
        }
        for(int i = Buffs.Count - 1; i >= 0; i--)
        {
            if(Buffs[i].DurType == Buff.DurationType.RealTime)
                Buffs.RemoveAt(i);
        }
        foreach(StatType Type in CurrentlyBuffedStats)
        {
            UpdateTrueStats(Type);
        }
    }
    public void ResetHex()
    {
        if(Hex)
        {
            if(!Benched)
            {
                Hex.GetComponent<Hex>().Taken = null;
            }
            else
            {
                Hex.GetComponent<BenchSlot>().Taken = null;
            }
        }
    }
    public void ResetFull()
    {
        for(int i = Buffs.Count - 1; i >= 0; i--)
        {
            if(Buffs[i].DurType == Buff.DurationType.RealTime)
                Buffs.RemoveAt(i);
        }
        GetStats();
        UpdateBaseStats();
        Shield = 0;
        ResetHex();
        Hex = OGHex;
        transform.position = (Vector2)Hex.transform.position;
        AttackCount = 0;
        AbilityCount = 0;
        foreach (Buff buff in Buffs)
        {
            UpdateTrueStats(buff.Type);
        }
        Hp = MaxHp;
        //Movement
        IsMoving = false;
        LerpProgress = 0;
        Path = (null, null);
        //Visuals
        Sprites.transform.rotation = Quaternion.identity;
        GetComponent<Animator>().Play("Idle");
        
        OnReset.Invoke();
    }
    public ref float StatTypeToStat(StatType Type)
    {
        if(Type == StatType.Hp)
        {
            return ref Hp;
        }
        else if(Type == StatType.MaxHp)
        {
            return ref MaxHp;
        }
        else if(Type == StatType.Mana)
        {
            return ref Mana;
        }
        else if (Type == StatType.AD)
        {
            return ref AtkDamage;
        }
        else if (Type == StatType.AP)
        {
            return ref AbilityPower;
        }
        else if (Type == StatType.AtkSpeed)
        {
            return ref AtkSpeed;
        }
        else if (Type == StatType.Armor)
        {
            return ref Armor;
        }
        else if (Type == StatType.MagicResist)
        {
            return ref MagicResist;
        }
        else if (Type == StatType.Movespeed)
        {
            return ref Movespeed;
        }
        else if (Type == StatType.DamageAmp)
        {
            return ref DamageAmp;
        }
        else if (Type == StatType.Durability)
        {
            return ref Durability;
        }
        else if (Type == StatType.Omnivamp)
        {
            return ref Omnivamp;
        }
        else if (Type == StatType.CritChance)
        {
            return ref CritChance;
        }
        else if (Type == StatType.CritDamage)
        {
            return ref CritDamage;
        }
        else
        {
            Debug.Log(Type);
            return ref AtkDamage;
        }
    }
    private float StatTypeToBaseStat(StatType Type)
    {
        if(Type == StatType.Hp)
        {
            return Stats.Hp * Mathf.Pow(Stats.HpScale, Star - 1);
        }
        else if(Type == StatType.MaxHp)
        {
            return Stats.MaxHp * Mathf.Pow(Stats.ADScale, Star - 1);
        }
        else if(Type == StatType.Mana)
        {
            return Stats.Mana;
        }
        else if (Type == StatType.AD)
        {
            return Stats.AtkDamage * Mathf.Pow(Stats.ADScale, Star - 1);
        }
        else if (Type == StatType.AP)
        {
            return Stats.AbilityPower;
        }
        else if (Type == StatType.AtkSpeed)
        {
            return Stats.AtkSpeed;
        }
        else if (Type == StatType.Armor)
        {
            return Stats.Armor;
        }
        else if (Type == StatType.MagicResist)
        {
            return Stats.MagicResist;
        }
        else if (Type == StatType.Movespeed)
        {
            return Stats.Movespeed;
        }
        else if (Type == StatType.DamageAmp)
        {
            return Stats.DamageAmp;
        }
        else if (Type == StatType.Durability)
        {
            return Stats.Durability;
        }
        else if (Type == StatType.Omnivamp)
        {
            return Stats.Omnivamp;
        }
        else if (Type == StatType.CritChance)
        {
            return Stats.CritChance;
        }
        else if (Type == StatType.CritDamage)
        {
            return Stats.CritDamage;
        }
        else
        {
            Debug.Log(Type);
            return Stats.AtkDamage;
        }
    }
    private void UpdateRealTimeBuffs()
    {
        if(Buffs.Count > 0)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                if(Buffs[i].DurType == Buff.DurationType.RealTime)
                {
                    Buffs[i].Duration -= Time.deltaTime;
                    if (Buffs[i].Duration <= 0)
                    {
                        RemoveBuff(Buffs[i]);
                    }
                }
            }
        }
    }
    public void UpdateMatchTimeBuffs()
    {
        if(Buffs.Count > 0)
        {
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {
                if(Buffs[i].DurType == Buff.DurationType.MatchTime)
                {
                    Buffs[i].Duration--;
                    if (Buffs[i].Duration <= 0)
                    {
                        RemoveBuff(Buffs[i]);
                    }
                }
            }
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
