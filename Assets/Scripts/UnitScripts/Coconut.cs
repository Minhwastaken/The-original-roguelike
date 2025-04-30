using UnityEngine;

public class Coconut : Unit
{
    public float BrokenPercent;
    public Sprite FullSprite, BrokenSprite;
    public SpriteRenderer SpriteRenderer;
    private Buff CoconutDurabilityBuff = new Buff("CoconutDurabilityBuff", 0, 30f, 0, StatType.Durability, Buff.DurationType.Permanent, false);
    protected override void Start()
    {
        base.Start();
        OnTakeDamage.AddListener(CheckBroken);
        OnReset.AddListener(Reset);
        GainBuff(CoconutDurabilityBuff);
    }
    private void CheckBroken()
    {
        if(Hp < MaxHp/100*BrokenPercent)
        {
            SpriteRenderer.sprite = BrokenSprite;
            AttackOverride = AttackPrefab;
            RemoveBuff(CoconutDurabilityBuff);
        }
    }
    private void Reset()
    {
        SpriteRenderer.sprite = FullSprite;
        AttackOverride = null;
    }
}
