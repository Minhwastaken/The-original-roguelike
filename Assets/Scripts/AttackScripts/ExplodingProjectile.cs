using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodingProjectile : TargetedProjectile
{
    public GameObject Explosion;
    public override void DamageDealtLogic()
    {
        Attacker.SpawnAttack(Explosion);
        DestroySelf();
    }
}
