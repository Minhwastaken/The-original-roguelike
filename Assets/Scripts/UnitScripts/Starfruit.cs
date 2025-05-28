using UnityEngine;

public class Starfruit : Unit
{
    protected override void Start()
    {
        base.Start();
        Buff AbilityAtkSpeedBuff = new Buff("StarfruitAbilityASBuff", 2, 0, 2f, StatType.AtkSpeed, Buff.DurationType.RealTime, false);
        OnAbilityUse.AddListener(() => GainBuff(AbilityAtkSpeedBuff));
        OnAbilityUse.AddListener(() => GainShield(2000));
        Debug.Log("Starfruit ability granted shield.");
    }
}
