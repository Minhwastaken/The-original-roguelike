using UnityEngine;
using UnityEditor;
using System.Collections;
public class Projectile : Attack
{
    [Header("Projectile")]
    public float Speed;
    public Vector2 Direction;
    public bool Targeted = false;
    public bool DestroyOnHit = true;
    [Header("Spawn On Hit")]
    public bool SpawnOnHit = false;
    public GameObject SpawnOnHitPrefab;
    protected override void Start()
    {
        base.Start();
        if(Targeted)
        {
            StartCoroutine(MoveTowardsTarget());
        }
        else
        {
            StartCoroutine(MoveStraight());
        }
    }
    private IEnumerator MoveStraight()
    {
        while(true)
        {
            transform.position += (Vector3)Direction * Speed * Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator MoveTowardsTarget()
    {
        while(true)
        {
            if(Target != null && Target.gameObject.activeSelf)
            {
                Direction = (Target.transform.position - transform.position).normalized;
                transform.position += (Vector3)Direction * Speed * Time.deltaTime;
            }
            else if (Attacker.GetComponent<Unit>().GetTarget())
            {
                Target = Attacker.GetComponent<Unit>().GetTarget().GetComponent<Unit>();
            }
            else
            {
                DestroySelf();
            }
            yield return null;
        }
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
    public override void AttackLogic()
    {
    }
    public override void DamageDealtLogic()
    {
        if(DestroyOnHit)
        {
            OnDamageDealt.AddListener(DestroySelf);
        }
        if(SpawnOnHit && SpawnOnHitPrefab != null)
        {
            Attacker.SpawnAttack(SpawnOnHitPrefab);
        }
    }
    /*
    [CustomEditor(typeof(Projectile))]
    public class ProjectileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Projectile projectile = (Projectile)target;

            projectile.Attacker = (Unit)EditorGUILayout.ObjectField("Attacker", projectile.Attacker, typeof(Unit), true);
            projectile.ADScale = EditorGUILayout.FloatField("AD Scale", projectile.ADScale);
            projectile.APScale = EditorGUILayout.FloatField("AP Scale", projectile.APScale);
            projectile.Speed = EditorGUILayout.FloatField("Speed", projectile.Speed);
            if(!projectile.Targeted)
                projectile.Direction = EditorGUILayout.Vector2Field("Direction", projectile.Direction);
            projectile.Targeted = EditorGUILayout.Toggle("Targeted", projectile.Targeted);
            if(projectile.Targeted)
            {
                projectile.Target = (Unit)EditorGUILayout.ObjectField("Target", projectile.Target, typeof(Unit), true);
            }
            projectile.DestroyOnHit = EditorGUILayout.Toggle("Destroy On Hit", projectile.DestroyOnHit);
            projectile.SpawnOnHit = EditorGUILayout.Toggle("Spawn On Hit", projectile.SpawnOnHit);
            if(projectile.SpawnOnHit)
                projectile.SpawnOnHitPrefab = (GameObject)EditorGUILayout.ObjectField("Spawn On Hit Prefab", projectile.SpawnOnHitPrefab, typeof(GameObject), true);
        }
    }*/
}
