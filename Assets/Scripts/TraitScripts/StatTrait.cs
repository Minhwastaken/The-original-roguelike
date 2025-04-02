using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Trait", menuName = "Trait/Stat Trait")]
public class StatTrait : TraitDefinition
{
    public List<Unit.StatType> StatTypes;
    public List<NestedList<int>> StatMagnitudes;
    Buff TraitBuff = new Buff("BruteBuff", 0, 0, 7, Unit.StatType.AD, Buff.DurationType.Permanent, false);
    public override void ApplyTrait(Unit unit, int level)
    {
        unit.GainBuff(TraitBuff);
        Debug.Log("Applying " + name + " trait to " + unit.name);
    }
    public override void UnapplyTrait(Unit unit)
    {
        unit.RemoveBuff(TraitBuff);
        Debug.Log("Remove " + name + " trait to " + unit.name);
    }
}
