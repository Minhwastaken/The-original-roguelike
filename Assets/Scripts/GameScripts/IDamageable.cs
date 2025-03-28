
public interface IDamageable
{
    void TakeDamage(float damage, Unit.DamageType type, Unit Attacker);
    void Die(Unit Attacker);
}
