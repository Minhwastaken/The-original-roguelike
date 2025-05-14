using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flat Stat Trait", menuName = "Trait/Flat Stat Trait")]
public class FlatStatTrait : TraitDefinition
{
    public List<Unit.StatType> StatTypes;
    public List<NestedList<float>> StatMagnitudes;
    Buff TraitBuff;
    public override void ApplyTrait(Unit unit, int level)
    {
        for(int i = 0; i < StatTypes.Count; i++)
        {
            TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, StatMagnitudes[level].innerList[i], 0, StatTypes[i], Buff.DurationType.Permanent, false);
            unit.GainBuff(TraitBuff);
        }
        //Debug.Log("Applying " + name + " trait to " + unit.name);
    }
    public override void UnapplyTrait(Unit unit, int level)
    {
        for(int i = 0; i < StatTypes.Count; i++)
        {
            TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, 0, StatTypes[i], Buff.DurationType.Permanent, false);
            unit.RemoveBuff(TraitBuff);
        }
        //Debug.Log("Remove " + name + " trait from " + unit.name);
    }
}
