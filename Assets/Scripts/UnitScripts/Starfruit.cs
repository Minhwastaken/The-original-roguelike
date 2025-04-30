using UnityEngine;

public class Starfruit : Unit
{
    protected override void Start()
    {
        base.Start();
        Buff AbilityAtkSpeedBuff = new Buff("StarfruitAbilityASBuff", 2, 0, 2f, Unit.StatType.AtkSpeed, Buff.DurationType.RealTime, false);
        OnAbilityUse.AddListener(() => GainBuff(AbilityAtkSpeedBuff));
    }
}
