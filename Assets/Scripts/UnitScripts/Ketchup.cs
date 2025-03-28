using UnityEngine;

public class Ketchup : Unit
{
    public GameObject EnhancedAttack;
    public int AttackCount, AttacksPerEnhance;
    protected override void OnStart()
    {
        GameManager.Instance.OnPrepStart.AddListener(() => { AttackOverride = null; AttackCount = 0;});
    }
    protected override void Attack()
    {
        if(Target)
        {
            Sprites.transform.up = (Vector2) (Target.transform.position - transform.position);
            AttackCount++;
            if(AttackCount >= AttacksPerEnhance)
            {
                AttackCount = 0;
                AttackOverride = EnhancedAttack;
            }
            Anim.Play("Attack");
        }
        AtkSpeed += Stats.AtkSpeed * 0.05f;
    }
}
