using System.Collections.Generic;
using UnityEngine;

public class MayoAbility : Attack
{
    public int SpawnNumber;
    public float SpreadAngle;
    public GameObject SpawnPrefab;
    protected override void Start()
    {
        List <GameObject> Enemies;
        if(Attacker.Team == 0)
        {
            Enemies = GridGen.Instance.Enemies;
        }
        else
        {
            Enemies = GridGen.Instance.Units;
        }
        SpawnNumber = Attacker.GetComponent<Mayo>().AbilityCount;
        for(int i = 0; i < SpawnNumber; i++)
        {
            GameObject spawn = Instantiate(SpawnPrefab, Attacker.AttackPoint.position, Quaternion.identity);
            spawn.GetComponent<TargetedProjectile>().Attacker = Attacker;
            spawn.GetComponent<TargetedProjectile>().Target = Enemies[Random.Range(0, Enemies.Count)].GetComponent<Unit>();
            spawn.transform.rotation = Quaternion.Euler(0, 0, Attacker.Sprites.transform.eulerAngles.z + Random.Range(SpreadAngle, -SpreadAngle));
            Debug.Log(Attacker.Sprites.transform.eulerAngles.z);
            spawn.transform.position += spawn.transform.up * 0.2f;
        }
    }
    public override void AttackLogic()
    {
        //Attacker.AtkSpeed += 0.05f;
    }
    public override void DamageDealtLogic()
    {
    }
}