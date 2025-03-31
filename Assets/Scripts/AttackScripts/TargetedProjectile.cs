using UnityEngine;

public class TargetedProjectile : Attack
{
    public float Speed, RotationSpeed, HitDistance = 1f, RotationSpeedIncreaseRate = 1f;
    protected override void Start()
    {
        base.Start();
        //transform.up = (Vector2)(Target.transform.position - transform.position);
    }
    private void Update()
    {
        if(Target && Target.gameObject.activeSelf)
        {
            RotationSpeed += RotationSpeedIncreaseRate * Time.deltaTime;
            Vector2 TrueDirection = ((Vector2)(Target.transform.position - transform.position)).normalized; 
            Vector2 Direction = (Vector2) Vector3.RotateTowards((Vector2)transform.up, TrueDirection, RotationSpeed * Time.deltaTime, 0.0f);
            transform.position += (Vector3)Direction * Speed * Time.deltaTime;
            transform.up = Direction;
        }
        else if (Attacker.GetComponent<Unit>().GetTarget())
        {
            Target = Attacker.GetComponent<Unit>().GetTarget().GetComponent<Unit>();
        }
        else
        {
            DestroySelf();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject == Target.gameObject)
        {
            TriggerAttack(other.gameObject.GetComponent<Unit>());
        }
    }
    public override void AttackLogic()
    {
    }
    public override void DamageDealtLogic()
    {
        DestroySelf();
    }
}
