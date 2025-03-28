using UnityEngine;

public class KetchupEnhancedAttack : Projectile
{
    public GameObject Explosion;
    public override void AttackLogic()
    {
        Attacker.AttackOverride = null;
    }
    public override void DamageDealtLogic()
    {
        base.DamageDealtLogic();
        Attacker.SpawnAttack(Explosion);
        DestroySelf();
    }
}
