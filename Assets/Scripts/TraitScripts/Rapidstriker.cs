using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Rapidstriker", menuName = "Trait/Rapidstriker")]
public class Rapidstrike : TraitDefinition
{
    public Unit.StatType StatType;
    public List<float> StatMagnitudes;
    public Buff RapidstrikeASBuff;
    public float BuffRepeats;
    private UnityAction Action;
    public override void ApplyTrait(Unit unit, int level)
    {
        RapidstrikeASBuff = new Buff("RapidstrikeASBuff", 1, 0, StatMagnitudes[level], StatType, Buff.DurationType.MatchTime, true);
        Action = () => GainRapidstrikeBuff(unit);
        unit.OnAttack.AddListener(Action);
    }

    public override void UnapplyTrait(Unit unit, int level)
    {
        unit.RemoveBuff(new Buff("RapidstrikeASBuff", 1, 0, 0, StatType, Buff.DurationType.MatchTime, true));
        unit.OnAttack.RemoveListener(Action);
    }
    private void GainRapidstrikeBuff(Unit unit)
    {
        if(unit.AttackCount <= BuffRepeats)
            unit.GainBuff(RapidstrikeASBuff);
    }
}
