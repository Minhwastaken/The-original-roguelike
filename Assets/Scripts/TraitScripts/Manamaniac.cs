using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Manamaniac", menuName = "Trait/Manamaniac")]
public class Manamaniac : TraitDefinition
{
    public List<float> StatMagnitudes;
    private UnityAction Action;
    public override void ApplyTrait(Unit unit, int level)
    {
        Action = () => unit.Mana += unit.ManaAmount * StatMagnitudes[level];
        unit.OnManaGain.AddListener(Action);
    }

    public override void UnapplyTrait(Unit unit)
    {
        if(Action == null) return;
        unit.OnManaGain.RemoveListener(Action);
    }
}
