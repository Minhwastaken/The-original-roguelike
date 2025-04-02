using UnityEngine;

public class KetchupEnhancedAttack : Projectile
{
    public GameObject Explosion;
    protected override void AttackLogic()
    {
        if(Attacker)
            Attacker.AttackOverride = null;
    }
    protected override void DamageDealtLogic()
    {
        base.DamageDealtLogic();
        if(Attacker)
            Attacker.SpawnAttack(Explosion);
        DestroySelf();
    }
}
