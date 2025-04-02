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
    protected override void DamageDealtLogic()
    {
        DestroySelf();
    }
}
