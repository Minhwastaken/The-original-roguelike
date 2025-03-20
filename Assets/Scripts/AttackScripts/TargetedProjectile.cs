using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedProjectile : Attack
{
    public float Speed;
    private void Update()
    {
        if(Target)
        {
            Vector2 Direction = ((Vector2)(Target.transform.position - transform.position)).normalized;
            transform.position += (Vector3)Direction * Speed * Time.deltaTime;
            transform.up = Direction;
            if(Vector2.Distance(transform.position, Target.transform.position) < 0.1f)
            {
                transform.position = Target.transform.position;
                TriggerAttack(Target);
            }
        }
        else if (Attacker.GetComponent<Unit>().GetTarget())
        {
            Target = Attacker.GetComponent<Unit>().GetTarget().GetComponent<Unit>();
        }
        else
        {
            DestroySelf();
        }
    }
    public override void AttackLogic()
    {
    }
    public override void DamageDealtLogic()
    {
        DestroySelf();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == GridGen.Instance.UnitLayer)
        {
            if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team)
            {
                transform.position = other.transform.position;
                TriggerAttack(other.gameObject.GetComponent<Unit>());
            }
        }
    }
}
