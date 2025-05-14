using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piercer", menuName = "Trait/Piercer")]
public class Piercer : TraitDefinition
{
    public List<float> StatMagnitudes;
    public override void ApplyTrait(Unit unit, int level)
    {
        unit.FlatArmorPierce = StatMagnitudes[level];
        unit.FlatMRPierce = StatMagnitudes[level];
    }
    public override void UnapplyTrait(Unit unit, int level)
    {
        unit.FlatArmorPierce -= StatMagnitudes[level];
        unit.FlatMRPierce -= StatMagnitudes[level];
    }
}