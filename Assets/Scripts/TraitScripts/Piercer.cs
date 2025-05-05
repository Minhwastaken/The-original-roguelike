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
        Debug.Log("Applying " + name + " trait to " + unit.name);
    }
    public override void UnapplyTrait(Unit unit)
    {
        unit.FlatArmorPierce = 0;
        unit.FlatMRPierce = 0;
        Debug.Log("Remove " + name + " trait from " + unit.name);
    }
}