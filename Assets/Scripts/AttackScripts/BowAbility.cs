using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAbility : Projectile
{
    public List<Attack> Attacks = new List<Attack>();
    protected override void Start()
    {
        //GetStats();
        StartCoroutine(DestroyWithLifespan());
        foreach (Attack Attack in Attacks)
        {
            Attack.Attacker = Attacker;
            Attack.Target = Target;
            if(Attack.GetComponent<Projectile>())
            {
                Attack.GetComponent<Projectile>().Direction = Attack.transform.up;
            }
            Attack.enabled = true;
        }
    }
}
