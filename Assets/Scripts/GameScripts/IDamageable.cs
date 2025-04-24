
public interface IDamageable
{
    void TakeDamage(float damage, Attack.DamageTypeEnum type, Unit Attacker);
    void Die(Unit Attacker);
}
