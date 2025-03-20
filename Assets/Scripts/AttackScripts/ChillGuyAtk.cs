using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillGuyAtk : Attack
{
    private void Start()
    {
        GetStats();
        TriggerAttack(Target);
    }
    public override void AttackLogic()
    {
        //Attacker.AtkSpeed += 0.05f;
    }
    public override void DamageDealtLogic()
    {
    }
}
