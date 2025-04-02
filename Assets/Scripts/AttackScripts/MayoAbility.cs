using System.Collections.Generic;
using UnityEngine;

public class MayoAbility : Attack
{
    public int InitialSpawnNumber, SpawnNumber;
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
        SpawnNumber = InitialSpawnNumber + Attacker.GetComponent<Mayo>().AbilityCount;
        for(int i = 0; i < SpawnNumber; i++)
        {
            GameObject spawn = Instantiate(SpawnPrefab, Attacker.AttackPoint.position, Quaternion.identity);
            spawn.GetComponent<Projectile>().Attacker = Attacker;
            spawn.GetComponent<Projectile>().Target = Enemies[Random.Range(0, Enemies.Count)].GetComponent<Unit>();
            spawn.transform.rotation = Quaternion.Euler(0, 0, Attacker.Sprites.transform.eulerAngles.z + Random.Range(SpreadAngle, -SpreadAngle));
            Debug.Log(Attacker.Sprites.transform.eulerAngles.z);
            spawn.transform.position += spawn.transform.up * SpawnRadius;
        }
    }
}