
public class Megumin : Unit
{
    protected override void Attack()
    {
        if(Target)
        {
            GainMana(10);
            Anim.Play("Attack");
        }
    }
}
