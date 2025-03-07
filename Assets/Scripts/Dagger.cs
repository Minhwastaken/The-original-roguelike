using UnityEngine;

public class Dagger : Unit
{
    public AnimationClip AbilityAnim;
    protected override void OnStart()
    {
        float AbilityAnimLength = AbilityAnim.length;
        Buff AbilityAtkSpeedBuff = new Buff("DaggerAbilityASBuff", AbilityAnimLength + 2, 0, 2f, Buff.StatType.AtkSpeed, Buff.DurationType.RealTime, false);
        OnAbilityUse.AddListener(() => GainBuff(AbilityAtkSpeedBuff));
    }
}
