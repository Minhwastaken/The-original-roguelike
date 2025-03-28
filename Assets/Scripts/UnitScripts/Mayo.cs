using UnityEngine;

public class Mayo : Unit
{
    public int AbilityCount = 0;
    protected override void OnStart()
    {
        OnAbilityUse.AddListener(() => {AbilityCount++;});
    }
}
