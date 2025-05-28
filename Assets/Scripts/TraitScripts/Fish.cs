using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Fish", menuName = "Trait/Fish")]
public class Fish : TraitDefinition
{
    public List<Unit.StatType> StatTypes;
    public List<NestedList<float>> StatMagnitudes;
    public List<bool> IsFlatBuff;
    public Buff RapidstrikeASBuff;
    public int CurrentBuffStack = 0;
    public int BuffRepeats;
    public float TimeBetweenBuffs;
    Buff TraitBuff;
    private UnityAction Action;
    public bool OnBattleStartAdded = false;
    private Dictionary<Unit, UnityAction> UnitActions = new Dictionary<Unit, UnityAction>();
    public override void ApplyTrait(Unit unit, int level)
    {
        Action = () => unit.StartCoroutine(ApplyFishBuffs(unit, level));
        if (!UnitActions.ContainsKey(unit))
        {
            UnitActions.Add(unit, Action);
        }
        else
        {
            UnitActions[unit] = Action;
        }
        GameManager.Instance.OnBattleStart.RemoveListener(Action);
        GameManager.Instance.OnBattleStart.AddListener(Action);
        OnBattleStartAdded = true;
        Debug.Log("Applying " + name + " trait to " + unit.name);
    }

    public override void UnapplyTrait(Unit unit, int level)
    {
        for (int i = 0; i < StatTypes.Count; i++)
        {
            TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, 0, StatTypes[i], Buff.DurationType.MatchTime, true);
            unit.RemoveBuff(TraitBuff);
        }
        if(Action == null) return;
        GameManager.Instance.OnBattleStart.RemoveListener(Action);
    }
    private IEnumerator ApplyFishBuffs(Unit unit, int level)
    {
        while (GameManager.Instance.State == GameManager.GameState.Battle && CurrentBuffStack < BuffRepeats)
        {
            for (int i = 0; i < StatTypes.Count; i++)
            {
                if (IsFlatBuff[i])
                {
                    TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, StatMagnitudes[level].innerList[i], 0, StatTypes[i], Buff.DurationType.MatchTime, true);
                }
                else
                {
                    TraitBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, StatMagnitudes[level].innerList[i], StatTypes[i], Buff.DurationType.MatchTime, true);
                }
                unit.GainBuff(TraitBuff);
            }

            unit.ScaleMult += StatMagnitudes[level].innerList[2];
            unit.UpdateScale();
            yield return new WaitForSeconds(TimeBetweenBuffs);
        }
        unit.ScaleMult -= StatMagnitudes[level].innerList[2] * (int) (GameManager.Instance.MatchTime() / TimeBetweenBuffs);
        unit.UpdateScale();
        yield return null;
    }
}
