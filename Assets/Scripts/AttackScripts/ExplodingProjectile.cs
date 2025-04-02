using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodingProjectile : TargetedProjectile
{
    public GameObject Explosion;
    protected override void DamageDealtLogic()
    {
        Attacker.SpawnAttack(Explosion);
        DestroySelf();
    }
}
