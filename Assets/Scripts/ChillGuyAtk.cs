using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillGuyAtk : Attack
{
    public override void AttackLogic()
    {
        Attacker.AtkSpeed += 0.05f;
    }
    public override void DamageDealtLogic()
    {
    }
}
