using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject Unit;
    public static int Count = 0;
    public void OnBuyButton()
    {
        if(GameManager.Instance.Gold >= Unit.GetComponent<Unit>().Stats.Cost)
        {
            Buy();
        }
    }
    public void Buy()
    {
        for(int i = 0; i < 9; i++)
        {
            GameObject BenchSlot = GridGen.Instance.BenchSlots[i];
            if(!BenchSlot.GetComponent<BenchSlot>().Taken)
            {
                //Debug.Log(BenchSlot.name);
                GameObject SpawnedUnit = Instantiate(Unit);
                GridGen.Instance.BenchedUnits.Add(SpawnedUnit);

                SpawnedUnit.transform.position = (Vector2) BenchSlot.transform.position;
                SpawnedUnit.GetComponent<Unit>().Benched = true;
                SpawnedUnit.GetComponent<Unit>().Hex = BenchSlot;
                SpawnedUnit.GetComponent<Unit>().OGHex = BenchSlot;

                BenchSlot.GetComponent<BenchSlot>().Taken = SpawnedUnit;
                gameObject.SetActive(false);

                Count++;
                SpawnedUnit.name = Unit.name + " *";
                CheckStarUp(SpawnedUnit);

                //GridGen.Instance.RemoveUnitFromPool(SpawnedUnit);
                GameManager.Instance.LoseGold(SpawnedUnit.GetComponent<Unit>().Stats.Cost);
                UI.Instance.GoldText.text = GameManager.Instance.Gold + "G";
                break;
            }
        }
    }

    public void CheckStarUp(GameObject Unit)
    {
        int UnitCopyCount = 0;
        GameObject[] UnitCopies = new GameObject[3];
        foreach(GameObject BenchedUnit in GridGen.Instance.BenchedUnits)
        {
            if(BenchedUnit)
            {
                Unit BenchedUnitScript = BenchedUnit.GetComponent<Unit>();
                if(BenchedUnitScript.Stats == Unit.GetComponent<Unit>().Stats && BenchedUnitScript.Star == Unit.GetComponent<Unit>().Star)
                {
                    if(UnitCopyCount == 3)
                    {
                        break;
                    }
                    UnitCopies[UnitCopyCount] = BenchedUnit;
                    UnitCopyCount++;
                }
            }
        }
        foreach(GameObject FieldedUnit in GridGen.Instance.Units)
        {
            Unit FieldedUnitScript = FieldedUnit.GetComponent<Unit>();
            if(FieldedUnitScript.Stats == Unit.GetComponent<Unit>().Stats && FieldedUnitScript.Star == Unit.GetComponent<Unit>().Star)
            {
                if(UnitCopyCount == 3)
                {
                    break;
                }
                UnitCopies[UnitCopyCount] = FieldedUnit;
                UnitCopyCount++;
            }
        }
        if(UnitCopyCount == 3)
        {
            Unit UpgradedUnitScript = UnitCopies[0].GetComponent<Unit>();
            UpgradedUnitScript.Star++;
            UnitCopies[0].name += "*";
            UnitCopies[0].GetComponent<Unit>().ResetFull();
            Mouse.Instance.HexLockIn(UnitCopies[0]);
            Debug.Log("Star Up! " + UpgradedUnitScript.Star + " Star ");
            for(int i = 1; i < 3; i++)
            {
                UnitCopies[i].GetComponent<Unit>().RemoveUnit();
            }

            if(UpgradedUnitScript.Star < 4)
            {
                CheckStarUp(UnitCopies[0]);
            }
        }
    }
}
