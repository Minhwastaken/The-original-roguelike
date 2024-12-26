using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEditor;
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
    public bool IsMoving = false;
    public float LerpProgress = 0, WalkCD = 2, WalkTimer = 0;
    private void Start() 
    {
        if(Team == 0)
            GridGen.Units.Add(gameObject);
        else
            GridGen.Enemies.Add(gameObject);
        GetStats(ref Hp,ref MaxHp,ref AtkDamage,ref AbilityPower,ref AtkSpeed,ref Range,ref Armor,ref MagicResist,ref DamageAmp,ref Durability,ref CritChance,ref CritDamage,ref Omnivamp,ref Movespeed);
    }
    private void Update()
    {
        if(!IsMoving)
        {
            if(WalkTimer > WalkCD)
            {
                if(Target())
                {
                    IsMoving = true;
                    WalkTimer = 0;
                }
            }
            else
            {
                WalkTimer+=Time.deltaTime;
            }
        }
        else
        {
            if(Target()!=null)
            {
                Debug.Log(Target());
                MoveTo(Pathfind(Hex, Target().GetComponent<Unit>().Hex)[0]);
            }
            else
            {
                Debug.Log("nah");
            }
        }
    }
    private void MoveTo(GameObject EndHex)
    {
        LerpProgress += Time.deltaTime * Movespeed / 100f;
        transform.position = Vector2.Lerp(Hex.transform.position, EndHex.transform.position, LerpProgress);
        if(LerpProgress > 0.9)
        {

            LerpProgress = 0;
            transform.position = EndHex.transform.position + new Vector3(0,0,-1);
            Hex.GetComponent<Hex>().Taken = false;
            Hex = EndHex;
            Hex.GetComponent<Hex>().Taken = true;
            IsMoving = false;
        }
    }
    public void GetStats(ref float Hp,ref float MaxHp,ref float AtkDamage,ref float AbilityPower,ref float AtkSpeed,ref float Range,ref float Armor,ref float MagicResist,ref float DamageAmp,ref float Durability,ref float CritChance,ref float CritDamage,ref float Omnivamp,ref float Movespeed)
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
                    Debug.Log("End reached");
                    return BuildPath(CameFrom, StartHex, EndHex);
                }
                if(!CameFrom.ContainsKey(AdjHex) && !AdjHex.GetComponent<Hex>().Taken) //Also check if it's occupied in the future.
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
        Debug.Log("Build Succeeded");
        return Path;
    }
    private GameObject Target()
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

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        for(int i = 0; i<360; i++)
        {
            Gizmos.DrawLine(transform.position + new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad), 0) * Stats.Range, transform.position + new Vector3(Mathf.Sin((i+1) * Mathf.Deg2Rad), Mathf.Cos((i+1) * Mathf.Deg2Rad), 0) * Stats.Range);
        }
    }
}
