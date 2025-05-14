using UnityEngine;

[CreateAssetMenu(menuName = "Item/Component")]
public class ItemComponent : Item
{
    public Buff ComponentBuff;
    public override void ApplyItem(Unit unit)
    {
        for(int i = 0; i < StatTypes.Count; i++)
        {
            if(IsFlatBuff[i])
            {
                ComponentBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, StatMagnitudes[i], 0, StatTypes[i], Buff.DurationType.Permanent, false);
            }
            else
            {
                ComponentBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, StatMagnitudes[i], StatTypes[i], Buff.DurationType.Permanent, false);
            }
            unit.GainBuff(ComponentBuff);
        }
    }

    public override void UnapplyItem(Unit unit)
    {
        for(int i = 0; i < StatTypes.Count; i++)
        {
            ComponentBuff = new Buff(name + StatTypes[i].ToString() + "Buff", 1, 0, 0, StatTypes[i], Buff.DurationType.Permanent, false);
            unit.RemoveBuff(ComponentBuff);
        }
    }
}
