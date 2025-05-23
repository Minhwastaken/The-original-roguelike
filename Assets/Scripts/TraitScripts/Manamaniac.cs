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
        Action = () => unit.GainMana(unit.ManaAmount * StatMagnitudes[level], true);
        unit.OnManaGain.AddListener(Action);
    }

    public override void UnapplyTrait(Unit unit, int level)
    {
        if(Action == null) return;
        unit.OnManaGain.RemoveListener(Action);
    }
}
