using System.Collections;
using UnityEngine;

public class Violet : Unit
{
    private Coroutine RollCoroutine;
    public float RollDuration;
    protected override void Start()
    {
        base.Start();
        OnAbilityUse.AddListener(StartRoll);
    }
    public void StartRoll()
    {
        if(RollCoroutine != null)
        {
            StopCoroutine(RollCoroutine);
        }
        RollCoroutine = StartCoroutine(Roll());
    }
    public IEnumerator Roll()
    {
        Debug.Log("theyseemerolln");
        GameObject SafestHex = Hex;
        int MaxDistFromEnemy = 0;
        int MinDist = 99;
        /*foreach(GameObject Enemy in GridGen.Instance.Enemies)
        {
            int MinDistToEnemy = HexDist(Hex, Enemy.GetComponent<Unit>().Hex);
            Debug.Log(Hex.name + " " + Enemy.GetComponent<Unit>().Hex.name + " " + HexDist(Hex, Enemy.GetComponent<Unit>().Hex) + " " + Enemy.name);
            if(MinDistToEnemy < MinDist)
            {
                MinDist = MinDistToEnemy;
            }
        }
        MaxDistFromEnemy = MinDist;*/

        foreach(GameObject AdjHex in Hex.GetComponent<Hex>().AdjHexes)
        {
            if(AdjHex.GetComponent<Hex>().Taken != null) continue;
            MinDist = 99;
            foreach(GameObject Enemy in GridGen.Instance.Enemies)
            {
                int MinDistToEnemy = HexDist(AdjHex, Enemy.GetComponent<Unit>().Hex);
                Debug.Log(AdjHex.name + " " + Enemy.GetComponent<Unit>().Hex.name + " " + HexDist(AdjHex, Enemy.GetComponent<Unit>().Hex) + " " + Enemy.name);
                if(MinDist > MinDistToEnemy)
                {
                    MinDist = MinDistToEnemy;
                }
            }
            if(MaxDistFromEnemy < MinDist && MinDist != 99)
            {
                MaxDistFromEnemy = MinDist;
                SafestHex = AdjHex;
            }
        }

        Debug.Log("Safest Hex: " + SafestHex.name + " " + MaxDistFromEnemy);
        IsMoving = false;
        Path = (null,null);
        LerpProgress = 0;
        Vector3 CurrentPos = transform.position;
        float SlerpProgress = 0;
        while(SlerpProgress < 0.95)
        {
            SlerpProgress += Time.deltaTime / RollDuration;
            transform.position = Vector3.Lerp(CurrentPos, SafestHex.transform.position, SlerpProgress);
            if(SlerpProgress >= 0.1f && Hex.GetComponent<Hex>().Taken == gameObject)
            {
                Hex.GetComponent<Hex>().Taken = null;
            }
            if(SlerpProgress > 0.8f && Hex != SafestHex)
            {
                Hex = SafestHex;
            }
            yield return null;
        }
        transform.position = SafestHex.transform.position + new Vector3(0,0,-1);
        yield return null;
        Debug.Log("Roll Ended");
    }
}
