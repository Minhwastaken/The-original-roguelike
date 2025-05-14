using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuker", menuName = "Trait/Nuker")]
public class Nuker : TraitDefinition
{
    [Header("Damage Amp & Attack Scale")]
    public List<NestedList<float>> StatMagnitudes;
    public Buff NukerDamageAmpBuff;
    public override void ApplyTrait(Unit unit, int level)
    {
        UnapplyTrait(unit, level - 1);
        NukerDamageAmpBuff = new Buff(name + Unit.StatType.DamageAmp, 1, StatMagnitudes[level].innerList[0] , 0, Unit.StatType.DamageAmp, Buff.DurationType.Permanent, false);
        unit.GainBuff(NukerDamageAmpBuff);
        unit.AttackScale *= 1 + StatMagnitudes[level].innerList[1];
    }
    public override void UnapplyTrait(Unit unit, int level)
    {
        if(level < 0) return;
        unit.RemoveBuff(new Buff(name + Unit.StatType.DamageAmp, 1, 0, 0, Unit.StatType.DamageAmp, Buff.DurationType.Permanent, false));
        unit.AttackScale /= 1 + StatMagnitudes[level].innerList[1];
    }
}
