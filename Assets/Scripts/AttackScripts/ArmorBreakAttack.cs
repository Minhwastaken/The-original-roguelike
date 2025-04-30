using UnityEngine;

public class ArmorBreakAttack : Attack
{
    public float FlatArmorBreakAmount = 0f, PercentageArmorBreakAmount = 0.5f;
    public float ArmorBreakDuration = 0f;
    public Buff ArmorBreakDebuff;
    protected override void Start()
    {
        base.Start();
        ArmorBreakDebuff = new Buff("ArmorBreak", ArmorBreakDuration, -FlatArmorBreakAmount, -PercentageArmorBreakAmount, Unit.StatType.Armor, Buff.DurationType.RealTime, false);
        OnAttack.AddListener(() => Target.GainBuff(ArmorBreakDebuff));
        TriggerAttack(Target);
    }
}
