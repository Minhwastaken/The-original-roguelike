using UnityEngine;

public class Dagger : Unit
{
    public AnimationClip AbilityAnim;
    protected override void Start()
    {
        base.Start();
        float AbilityAnimLength = AbilityAnim.length;
        Buff AbilityAtkSpeedBuff = new Buff("DaggerAbilityASBuff", AbilityAnimLength + 2, 0, 2f, Unit.StatType.AtkSpeed, Buff.DurationType.RealTime, false);
        OnAbilityUse.AddListener(() => GainBuff(AbilityAtkSpeedBuff));
    }
}
