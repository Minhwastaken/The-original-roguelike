using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Unit
{
    [Header("Passive")]
    [SerializeField] private float[] AbilityHealAPScale = new float[3] { 1, 2, 3 };
    private void Ability()
    {
        Buff AppleKillBuff = new Buff("AppleKillBuff", 0, 0, 0.1f, Unit.StatType.AD, Buff.DurationType.Permanent, true);
        GainBuff(AppleKillBuff);
        Heal(this, AbilityPower * AbilityHealAPScale[Star - 1]);
    }
    protected override void Start()
    {
        base.Start();
        OnKill.AddListener(Ability);
    }
}
