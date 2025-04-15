using System.Collections.Generic;
using UnityEngine;

public class SpawnerAttack : Attack
{
    public int InitialSpawnNumber;
    public bool Scale, TargetRandomly;
    public int SpawnNumber;
    public float SpreadAngle, SpawnRadius;
    public GameObject SpawnPrefab;
    protected override void Start()
    {
        StartCoroutine(DestroyWithLifespan());
        List <GameObject> Enemies;
        if(Attacker.Team == 0)
        {
            Enemies = GridGen.Instance.Enemies;
        }
        else
        {
            Enemies = GridGen.Instance.Units;
        }
        if(Attacker.GetComponent<Unit>())
        {
            SpawnNumber = InitialSpawnNumber;
            if(Scale) 
            {
                SpawnNumber += Attacker.GetComponent<Unit>().AbilityCount;
            }
        }
        for(int i = 0; i < SpawnNumber; i++)
        {
            GameObject spawn = Instantiate(SpawnPrefab, Attacker.transform.position, Quaternion.identity);
            if(Attacker.AttackPoint)
            {
                spawn.transform.position = Attacker.AttackPoint.position;
            }
            spawn.GetComponent<Projectile>().Attacker = Attacker;
            if(TargetRandomly)
                spawn.GetComponent<Projectile>().Target = Enemies[Random.Range(0, Enemies.Count)].GetComponent<Unit>();
            else if(Attacker.Target)
                spawn.GetComponent<Projectile>().Target = Attacker.Target.GetComponent<Unit>();
            else if(Attacker.GetTarget())
                spawn.GetComponent<Projectile>().Target = Attacker.GetTarget().GetComponent<Unit>();
            spawn.transform.rotation = Quaternion.Euler(0, 0, Attacker.Sprites.transform.eulerAngles.z + Random.Range(SpreadAngle, -SpreadAngle));
            spawn.transform.position += spawn.transform.up * SpawnRadius;
        }
    }
}