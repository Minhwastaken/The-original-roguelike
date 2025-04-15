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
            if(TurnOnAttack)
            {
                Sprites.transform.up = (Vector2) (Target.transform.position - transform.position);
            }
            AttackCount++;
            if(AttackCount >= AttacksPerEnhance)
            {
                AttackCount = 0;
                AttackOverride = EnhancedAttack;
            }
            if(AttackOverride &&  Anim.HasState(0, Animator.StringToHash("AttackOverride")))
            {
                Anim.Play("AttackOverride");
            }
            else
            {
                Anim.Play("Attack");
            }
        }
        AtkSpeed += Stats.AtkSpeed * 0.05f;
    }
}
