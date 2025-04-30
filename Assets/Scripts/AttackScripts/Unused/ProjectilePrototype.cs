using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePrototype : Attack
{
    public float Speed;
    public Vector2 Direction;
    public void Update()
    {
        transform.position += (Vector3)Direction * Speed * Time.deltaTime;
        Lifespan-=Time.deltaTime;
        if(Lifespan<=0)
            DestroySelf();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(GameManager.Instance.State != GameManager.GameState.Battle)
            return;
        if(other.gameObject.layer == GridGen.Instance.UnitLayer)
        {
            if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team)
            {
                TriggerAttack(other.gameObject.GetComponent<Unit>());
            }
        }
    }
    protected override void DamageDealtLogic()
    {
        OnDamageDealt.AddListener(DestroySelf);
    }
}
