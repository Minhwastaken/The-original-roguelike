using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Diagnostics.Tracing;

public class Unit : MonoBehaviour
{
    public int Team;
    public UnitStats Stats;
    public float Hp, MaxHp;
    public float AtkDamage, AbilityPower;
    public float AtkSpeed;
    public float Range;
    public float Armor, MagicResist; 
    public float Movespeed;
    public float DamageAmp, Durability;
    public float Omnivamp;
    public float CritChance, CritDamage;
    public GameObject Hex, OGHex;
    public GameObject AttackGO;
    public bool IsMoving = false, Benched;
    public float LerpProgress = 0, WalkCD = 2, WalkTimer = 0;
    private Animator Anim;
    public AnimationClip AtkAnim, WalkAnim, IdleAnim;
    private GameObject Target, TargetHex;
    public enum DamageType {AD, AP, True};
    public (GameObject From, GameObject To) Path;
    private void Start() 
    {
        GameManager.Instance.OnBattleStart.AddListener(() => StartCoroutine(Action()));
        /*if(Hex)
        {
            OGHex = Hex;
            Hex.GetComponent<Hex>().Taken = gameObject;
        }*/
        if(Team == 1)
            GridGen.Instance.Enemies.Add(gameObject);
        GetStats();
        Anim = GetComponent<Animator>();
    }
    private void Update()
    {
        /*
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
                }
                //Debug.Log("Moving to next target");
                MoveTo(TargetHex);
            }
        }
        else if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //Debug.Log("Target attackable");
            Attack();
        }
        */
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
                        MoveTo(TargetHex);
                    }
                }
                else if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    //Debug.Log("Target attackable");
                    Attack();
                }
                yield return null;
            }
        }
    }
    private void Attack()
    {
        if(Target)
        {
            transform.up = (Vector2) (Target.transform.position - transform.position);
            Anim.Play(AtkAnim.name);
        }
    }
    public void SpawnAttack()
    {
        GameObject Attack = Instantiate(AttackGO, transform.position + transform.up, transform.rotation * Quaternion.Euler(0,0,45));
        Attack.GetComponent<Attack>().Attacker = gameObject;
        Attack.GetComponent<Attack>().Target = Target;
    }
    private void MoveTo(GameObject EndHex)
    {
       // Debug.Log(Hex + " " + EndHex);
        LerpProgress += Time.deltaTime * Movespeed / 100f;
        transform.position = Vector2.Lerp(Hex.transform.position, EndHex.transform.position, LerpProgress);
        if(LerpProgress >= 0.1f && Hex.GetComponent<Hex>().Taken == gameObject)
        {
            Hex.GetComponent<Hex>().Taken = null;
        }
        if(LerpProgress > 0.95f)
        {
            LerpProgress = 0;
            transform.position = EndHex.transform.position + new Vector3(0,0,-1);
            Hex = EndHex;
            IsMoving = false;
            Path = (null, null);
        }
    }
    public void GetStats()
    {
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

        AttackGO = Stats.Attack;
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
    private GameObject GetTarget()
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
            if(Target == null)
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
            if(Target == null)
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
        if(Target)
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
    
    public void TakeDamage(float damage, DamageType type)
    {
        Hp -= damage;
        if(Hp <= 0)
        {
            if(Team == 0)
            {
                GridGen.Instance.Units.Remove(gameObject);
                if(GridGen.Instance.Units.Count == 0)
                {
                    GameManager.Instance.ChangeState();
                }
            }
            else
            {
                GridGen.Instance.Enemies.Remove(gameObject);
                if(GridGen.Instance.Enemies.Count == 0)
                {
                    GameManager.Instance.ChangeState();
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        for(int i = 0; i<360; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad), 0) * Stats.Range, transform.position + new Vector3(Mathf.Sin((i+1) * Mathf.Deg2Rad), Mathf.Cos((i+1) * Mathf.Deg2Rad), 0) * Stats.Range);
        }
        Gizmos.color = Color.blue;
        if(Path.From && Path.To)
            Gizmos.DrawLine(Path.From.transform.position, Path.To.transform.position);
    }
}
