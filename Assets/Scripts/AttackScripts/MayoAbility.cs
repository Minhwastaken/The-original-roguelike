using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayoAbility : Attack
{
    public int SpawnNumber;
    public GameObject SpawnPrefab;
    private void Start()
    {
        SpawnNumber = Attacker.GetComponent<Mayo>().AbilityCount;
        for(int i = 0; i < SpawnNumber; i++)
        {
            GameObject spawn = Instantiate(SpawnPrefab, HitEffectPos.position, Quaternion.identity);
            spawn.GetComponent<TargetedProjectile>().Attacker = Attacker;
            spawn.GetComponent<TargetedProjectile>().Target = GridGen.Instance.Enemies[Random.Range(0, GridGen.Instance.Enemies.Count)].GetComponent<Unit>();
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