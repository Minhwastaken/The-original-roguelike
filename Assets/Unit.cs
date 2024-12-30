using System.Collections.Generic;
using UnityEngine;

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
    public GameObject Hex;
    public GameObject AttackGO;
    public bool IsMoving = false;
    public float LerpProgress = 0, WalkCD = 2, WalkTimer = 0;
    private Animator Anim;
    public AnimationClip AtkAnim, WalkAnim, IdleAnim;
    private GameObject Target, TargetHex;
    public enum DamageType {AD, AP, True};
    private void Start() 
    {
        Hex.GetComponent<Hex>().Taken = true;
        if(Team == 0)
            GridGen.Units.Add(gameObject);
        else
            GridGen.Enemies.Add(gameObject);
        GetStats();
        Anim = GetComponent<Animator>();
    }
    private void Update()
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
                }
                //Debug.Log("Moving to next target");
                MoveTo(TargetHex);
            }
        }
        else
        {
            if(AttackableTarget())
            {
                //Debug.Log("Target attackable");
                if(Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    Attack();
                }
            }
            else if(GetTargetInRange() && Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                //Debug.Log("Target dead or no longer in range, switching targets");
                Target = GetTargetInRange();
                Attack();
            }
        }
    }
    private void Attack()
    {
        if(GetTarget())
        {
            transform.up = (Vector2) (GetTarget().transform.position - transform.position);
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
        if(EndHex.gameObject.GetComponent<Hex>().Reserved == null)
            EndHex.gameObject.GetComponent<Hex>().Reserved = gameObject;
        LerpProgress += Time.deltaTime * Movespeed / 100f;
        transform.position = Vector2.Lerp(Hex.transform.position, EndHex.transform.position, LerpProgress);
        if(LerpProgress > 0.99f)
        {
            LerpProgress = 0;
            transform.position = EndHex.transform.position + new Vector3(0,0,-1);
            Hex.GetComponent<Hex>().Taken = false;
            Hex = EndHex;
            Hex.GetComponent<Hex>().Taken = true;
            EndHex.gameObject.GetComponent<Hex>().Reserved = null;
            IsMoving = false;
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
        Queue<GameObject> BFSQueue = new Queue<GameObject>();
        Dictionary<GameObject, GameObject> CameFrom = new Dictionary<GameObject, GameObject>();
        BFSQueue.Enqueue(StartHex);
        //CameFrom.Add(StartHex, StartHex);

        while(BFSQueue.Count > 0)
        {
            GameObject CurHex = BFSQueue.Dequeue();

            /*if(CurHex == EndHex)
            {
                Debug.Log("End reached");
                return BuildPath(CameFrom, StartHex, EndHex);
            }*/

            foreach(GameObject AdjHex in CurHex.GetComponent<Hex>().AdjHexes)
            {
                if(AdjHex == EndHex)
                {
                    CameFrom.Add(AdjHex, CurHex);
                    //Debug.Log("End reached");
                    return BuildPath(CameFrom, StartHex, EndHex);
                }

                Hex AdjHexHex = AdjHex.GetComponent<Hex>();
                
                bool CheckReserve = false;
                if(AdjHexHex.Reserved == null || AdjHexHex.Reserved == gameObject)
                    CheckReserve = true;
                else if(AdjHexHex.Reserved.GetComponent<Unit>().Team != Team)
                    CheckReserve = true;

                if(!CameFrom.ContainsKey(AdjHex) && !AdjHexHex.Taken && CheckReserve) //Also check if it's occupied in the future.
                {
                    BFSQueue.Enqueue(AdjHex);
                    //CameFrom[AdjHex] = CurHex;
                    CameFrom.Add(AdjHex, CurHex);
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
            Targets = GridGen.Enemies;    
        }
        else
        {
            Targets = GridGen.Units;
        }
        foreach(GameObject Target in Targets)
        {
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
            Targets = GridGen.Enemies;    
        }
        else
        {
            Targets = GridGen.Units;
        }

        if(Targets.Count == 0)
            return null;

        foreach(GameObject Target in Targets)
        {
            float Dist = Vector2.Distance(transform.position, Target.transform.position);
            if(Dist < MinDist && Dist < Range)
            {
                ClosestEnemy = Target;
                MinDist = Dist;
            }
        }
        return ClosestEnemy;
    }
    private bool AttackableTarget()
    {
        if(Target && Vector2.Distance(transform.position, Target.transform.position) < Range)
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(float damage, DamageType type)
    {
        Hp -= damage;
        if(Hp <= 0)
        {
            if(Team == 0)
                GridGen.Units.Remove(gameObject);
            else
                GridGen.Enemies.Remove(gameObject);
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
    }
}
