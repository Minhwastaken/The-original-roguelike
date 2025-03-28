using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Trait")]
public abstract class TraitDefinition : ScriptableObject
{
    public enum Traits {Weapon, Fruit, Clothes, Staff, Condiment, Brute, Rapidstriker, Nuker, Shredder, Paladdin,
                        Thicc}
    public Traits TraitType;
    public string Name;
    public List<int> Thresholds;
    public abstract void ApplyTrait(Unit unit, int level);
    public abstract void UnapplyTrait(Unit unit);
}
