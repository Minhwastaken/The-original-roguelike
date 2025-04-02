using System.Collections.Generic;
using UnityEngine;

public class AOEAttack : Attack
{
    public float LingerTime = 0.1f;
    public int TickNumber = 1;
    public float TickInterval = 0.2f;
    public bool SpawnAtTarget;
    public int TickCount = 0;
    private float TickTimer = 0f;
    public List<Unit> HitEnemies = new List<Unit>();
    public List<GameObject> ParticleSystems = new List<GameObject>();
    private void Update()
    {
        TickTimer += Time.deltaTime;
        if(TickNumber > 1 && TickTimer >= TickInterval)
        {
            TickTimer = 0f;
            if(HitEnemies.Count > 0 && GameManager.Instance.State != GameManager.GameState.Prep)
            {
                for(int i = 0; i < HitEnemies.Count; i++)
                {
                    TriggerAttack(HitEnemies[i]);
                }
            }
            TickCount++;
            if(TickCount >= TickNumber || GameManager.Instance.State == GameManager.GameState.Prep)
            {
                if(ParticleSystems.Count > 0)
                {
                    foreach(GameObject ps in ParticleSystems)
                    {
                        ps.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }
                }
                HitEnemies.Clear();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.name);
        if(TickNumber == 1)
        {
            if(LingerTime < 0 || GameManager.Instance.State == GameManager.GameState.Prep)
                return;
            if(other.gameObject.layer == GridGen.Instance.UnitLayer)
            {
                if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team)
                {
                    TriggerAttack(other.gameObject.GetComponent<Unit>());
                }
            }
        }
        else if(TickCount < TickNumber)
        {
            if(other.gameObject.layer == GridGen.Instance.UnitLayer)
            {
                if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team && !HitEnemies.Contains(other.gameObject.GetComponent<Unit>()))
                {
                    HitEnemies.Add(other.gameObject.GetComponent<Unit>());
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == GridGen.Instance.UnitLayer)
        {
            if(other.gameObject.GetComponent<Unit>().Team != Attacker.Team && HitEnemies.Contains(other.gameObject.GetComponent<Unit>()))
            {
                HitEnemies.Remove(other.gameObject.GetComponent<Unit>());
            }
        }
    }
}
