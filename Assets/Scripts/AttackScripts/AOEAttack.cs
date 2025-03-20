using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Attack
{
    public float LingerTime = 0.1f;
    public bool SpawnAtTarget;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if(Duration > LingerTime || GameManager.Instance.State == GameManager.GameState.Prep)
            return;
        if(other.gameObject.layer == GridGen.Instance.UnitLayer)
        {
            if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team)
            {
                TriggerAttack(other.gameObject.GetComponent<Unit>());
            }
        }
    }
    public override void AttackLogic()
    {
    }
    public override void DamageDealtLogic()
    {
    }
}
