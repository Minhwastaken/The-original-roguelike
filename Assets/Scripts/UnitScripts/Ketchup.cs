using UnityEngine;

public class Ketchup : Unit
{
    public GameObject EnhancedAttack;
    public int AttackCount, AttacksPerEnhance;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.OnPrepStart.AddListener(() => {AttackOverride = null; AttackCount = 0;});
    }
    protected override void Attack()
    {
        if(Target)
        {
            AttackCount++;
            if(AttackCount >= AttacksPerEnhance)
            {
                AttackCount = 0;
                AttackOverride = EnhancedAttack;
            }
        }
        //AtkSpeed += Stats.AtkSpeed * 0.05f;
        base.Attack();
    }
}
