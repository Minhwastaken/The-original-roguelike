using UnityEngine;

public class KetchupEnhancedAttack : Projectile
{
    public GameObject Explosion;
    public override void AttackLogic()
    {
        if(Attacker)
            Attacker.AttackOverride = null;
    }
    public override void DamageDealtLogic()
    {
        base.DamageDealtLogic();
        if(Attacker)
            Attacker.SpawnAttack(Explosion);
        DestroySelf();
    }
}
