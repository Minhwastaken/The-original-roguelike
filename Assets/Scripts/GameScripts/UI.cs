using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public GameObject Shop, RollChanceText, RerollButton, ShopButton, TraitHolder, ItemHolder;
    public bool ShopLocked = false;
    public int[,] RollChance = new int[11, 5] 
    {
        {100, 0, 0, 0, 0}, 
        {100, 0, 0, 0, 0}, 
        {75, 25, 0, 0, 0}, 
        {55, 30, 15, 0, 0}, 
        {45, 33, 20, 2, 0},
        {30, 40, 25, 5, 0},
        {19, 30, 35, 10, 1},
        {18, 25, 32, 22, 3},
        {10, 20, 25, 35, 10},
        {5, 10, 20, 40, 25},
        {1, 2, 12, 50, 35}
    };
    public Text GoldText, XpText, StageText, LevelText;
    public List<GameObject> BuyButtons, TraitVisuals;
    public List<Text> RollChanceTexts;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Reroll();
        GameManager.Instance.OnPrepStart.AddListener(RollIfUnlocked);
    }
    public void OnRerollButton()
    {
        if(GameManager.Instance.Gold >= GameManager.Instance.RerollCost)
        {
            Reroll();
            GameManager.Instance.LoseGold(GameManager.Instance.RerollCost);
        }
    }
    public void Reroll()
    {
        foreach(GameObject Button in BuyButtons)
        {
            if(!Button.activeSelf)
            {
                Button.SetActive(true);
            }
            else if(Button.GetComponent<Button>().Unit)
            {
                    GridGen.Instance.AddUnitToPool(Button.GetComponent<Button>().Unit);
            }
            int UnitCost = RollForCost();
            //Debug.Log("Rolling for cost " + (UnitCost + 1));
            GameObject RandomUnit = RandomUnitFromPool(UnitCost);
            GridGen.Instance.RemoveUnitFromPool(RandomUnit);
            Button.GetComponent<Button>().Unit = RandomUnit;
            Button.transform.GetChild(0).GetComponent<Text>().text = RandomUnit.GetComponent<Unit>().Stats.Name + " (" +  RandomUnit.GetComponent<Unit>().Stats.Cost.ToString() + ")";
        }
    }
    public int RollForCost()
    {
        int TotalAvailableCostWeight = 0;
        List<bool> AvailableCosts = new List<bool> {true, true, true, true, true};
        for (int i = 0; i < GridGen.MaxUnitCost; i++)
        {
            if (RandomUnitFromPool(i + 1))
            {
                TotalAvailableCostWeight += RollChance[GameManager.Instance.Level - 1, i];
            }
            else
            {
                AvailableCosts[i] = false;
            }
        }
        int Roll = Random.Range(0, TotalAvailableCostWeight);
        //Debug.Log("Rolled a " + Roll);
        int Total = 0;
        for(int i = 0; i < GridGen.MaxUnitCost; i++)
        {
            if(!AvailableCosts[i])
            {
                continue;
            }
            if (GameManager.Instance.Level < 11)
            {
                Total += RollChance[GameManager.Instance.Level - 1, i];
            }
            else
            {
                Total += RollChance[10, i];
            }
            if(Roll < Total)
            {
                return i + 1;
            }
        }
        return 0;
    }
    public GameObject RandomUnitFromPool(int Cost)
    {
        int Sum = 0;
        List<(GameObject Unit, int Count)> UnitPoolOfCost = GridGen.Instance.UnitPool[Cost - 1];
        foreach ((GameObject Unit, int Count) Tuple in UnitPoolOfCost)
        {
            if(Tuple.Count <= 0)
            {
                continue;
            }
            Sum += Tuple.Count;
        }
        if (Sum == 0)
        {
            Debug.Log("No units of cost " + Cost);
            return null;
        }
        int Roll = Random.Range(0, Sum);
        for (int i = 0; i < UnitPoolOfCost.Count; i++)
        {
            if(UnitPoolOfCost[i].Count <= 0)
            {
                continue;
            }
            Roll -= UnitPoolOfCost[i].Count;
            if (Roll <= 0)
            {
                return UnitPoolOfCost[i].Unit;
            }
        }
        Debug.LogError("Couldn't find a unit of cost " + Cost);
        return null;
    }
    public void UpdateRollChance()
    {
        if(GameManager.Instance.Level < 11)
        {
            for(int i = 0; i < GridGen.MaxUnitCost; i++)
            {
                RollChanceTexts[i].text = RollChance[GameManager.Instance.Level - 1, i] + "%";
            }
        }
        else
        {
            for(int i = 0; i < GridGen.MaxUnitCost; i++)
            {
                RollChanceTexts[i].text = RollChance[10, i] + "%";
            }
        }
    }
    public void ToggleShop()
    {
        if(Shop.activeSelf)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }
    private void OpenShop()
    {
        Shop.SetActive(true);
        //RollChanceText.SetActive(true);
        RerollButton.SetActive(true);
    }
    private void CloseShop()
    {
        Shop.SetActive(false);
        //RollChanceText.SetActive(false);
        RerollButton.SetActive(false);
    }
    public void ToggleShopLock()
    {
        ShopLocked = !ShopLocked;
    }
    private void RollIfUnlocked()
    {
        UpdateRollChance();
        if(!ShopLocked)
        {
            Reroll();
        }
        else
        {
            ShopLocked = false;
        }
    }
    public void SwitchToTraits()
    {
        TraitHolder.SetActive(true);
        ItemHolder.SetActive(false);
    }
    public void SwitchToItems()
    {
        TraitHolder.SetActive(false);
        ItemHolder.SetActive(true);
    }
}
