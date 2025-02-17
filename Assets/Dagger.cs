using UnityEngine;

public class Dagger : Unit
{
    public AnimationClip AbilityAnim;
    protected override void OnStart()
    {
        float AbilityAnimLength = AbilityAnim.length;
        Buff AbilityAtkSpeedBuff = new Buff("DaggerAbility", AbilityAnimLength + 2, 0, 2f, Buff.BuffType.AtkSpeed, false);
        OnAbilityUse.AddListener(() => GainBuff(AbilityAtkSpeedBuff));
    }
}
