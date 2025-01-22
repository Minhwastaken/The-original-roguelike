using UnityEngine;

public class Button : MonoBehaviour
{
    public static int Count = 0;
    public void Buy(GameObject Unit)
    {
        foreach(GameObject BenchSlot in GridGen.Instance.BenchSlots)
        {
            if(BenchSlot.GetComponent<BenchSlot>().Taken == null)
            {
                GameObject SpawnedUnit = Instantiate(Unit);
                GridGen.Instance.BenchedUnits.Add(SpawnedUnit);
                SpawnedUnit.transform.position = BenchSlot.transform.position;
                SpawnedUnit.GetComponent<Unit>().Benched = true;
                Unit.GetComponent<Unit>().Hex = BenchSlot;
                BenchSlot.GetComponent<BenchSlot>().Taken = Unit;
                gameObject.SetActive(false);

                Count++;
                SpawnedUnit.name = Unit.name + " " + Count;
                break;
            }
        }
    }
}
