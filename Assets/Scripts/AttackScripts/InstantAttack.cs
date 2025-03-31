using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantAttack : Attack
{
    protected override void Start()
    {
        base.Start();
        TriggerAttack(Target);
    }
    public override void AttackLogic()
    {
        //Attacker.AtkSpeed += 0.05f;
    }
    public override void DamageDealtLogic()
    {
        DestroySelf();
    }
}
