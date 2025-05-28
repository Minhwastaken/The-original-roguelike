using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Sharpshot", menuName = "Trait/Sharpshot")]
public class Sharpshot : TraitDefinition
{
    public List<Unit.StatType> StatTypes;
    public List<NestedList<float>> StatMagnitudes;
    Buff TraitBuff;
    UnityAction AddExecuteToAttackAction;
    private Dictionary<Unit, UnityAction> unitExecuteActions = new Dictionary<Unit, UnityAction>();
    public override void ApplyTrait(Unit unit, int level)
    {
        for (int i = 0; i < StatTypes.Count; i++)
        {
            TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, StatMagnitudes[level].innerList[i], 0, StatTypes[i], Buff.DurationType.Permanent, false);
            unit.GainBuff(TraitBuff);
        }
        AddExecuteToAttackAction = () => AddExecuteToAttack(unit, level);
        if (!unitExecuteActions.ContainsKey(unit))
        {
            unitExecuteActions.Add(unit, AddExecuteToAttackAction);
        }
        else
        {
            unitExecuteActions[unit] = AddExecuteToAttackAction;
        }
        Debug.Log("removing and applying level " + level + " execute to attack for " + unit.name);
        unit.OnSpawnAttack.RemoveListener(AddExecuteToAttackAction);
        Debug.Log(unit.OnSpawnAttack.GetPersistentEventCount() + " listeners on spawn attack for " + unit.name);
        unit.OnSpawnAttack.AddListener(AddExecuteToAttackAction);
    }

    public override void UnapplyTrait(Unit unit, int level)
    {
        for (int i = 0; i < StatTypes.Count; i++)
        {
            TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, 0, StatTypes[i], Buff.DurationType.Permanent, false);
            unit.RemoveBuff(TraitBuff);
        }
        if( unitExecuteActions.ContainsKey(unit))
        {
            unit.OnSpawnAttack.RemoveListener(unitExecuteActions[unit]);
            unitExecuteActions.Remove(unit);
        }
        Debug.Log("removing level " + level + " execute to attack for " + unit.name);
    }
    public void AddExecuteToAttack(Unit unit, int level)
    {
        unit.SpawnedAttack.GetComponent<Attack>().OnDamageDealt.AddListener(() =>
        {
            if (unit.Target == null || unit.Target.GetComponent<Unit>() == null)
            {
                return;
            }
            Unit target = unit.Target.GetComponent<Unit>();
            if (target.Hp / target.MaxHp * 100 <= StatMagnitudes[level].innerList[2])
            {
                target.Die(unit);
            }
        });
    }
}
