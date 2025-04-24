using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public GameObject Shop, RollChanceText, RerollButton, ShopButton, TraitHolder;
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
    public Text GoldText, XpText, StageText;
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
            Button.SetActive(true);
            int UnitCost = RollForCost();
            //Debug.Log("Rolling for cost " + (UnitCost + 1));
            GameObject RandomUnit = RandomUnitFromPool(UnitCost);
            Button.GetComponent<Button>().Unit = RandomUnit;
            Button.transform.GetChild(0).GetComponent<Text>().text = RandomUnit.GetComponent<Unit>().Stats.Cost.ToString();
        }
    }
    public int RollForCost()
    {
        int Roll = Random.Range(0, 100);
        int Total = 0;
        for(int i = 0; i < 5; i++)
        {
            if(GameManager.Instance.Level < 11)
            {
                for(int y = 0; y < 5; y++)
                {
                    Total += RollChance[GameManager.Instance.Level - 1, i];
                }
            }
            else
            {
                for(int y = 0; y < 5; y++)
                {
                    Total += RollChance[10, i];
                }
            }
            
            if(Roll < Total)
            {
                return i;
            }
        }
        return 0;
    }
    public GameObject RandomUnitFromPool(int Cost)
    {
        int Sum = 0;
        foreach((GameObject Unit, int Count) Tuple in GridGen.Instance.UnitPool[Cost])
        {
            Sum += Tuple.Count;
        }
        int Roll = Random.Range(0, Sum);
        foreach((GameObject Unit, int Count) Tuple in GridGen.Instance.UnitPool[Cost])
        {
            Roll -= Tuple.Count;
            if(Roll <= 0)
            {
                return Tuple.Unit;
            }
        }
        Debug.LogError("Couldn't find a unit of cost " + Cost);
        return null;
    }
    public void UpdateRollChance()
    {
        if(GameManager.Instance.Level < 11)
        {
            for(int i = 0; i < 5; i++)
            {
                RollChanceTexts[i].text = RollChance[GameManager.Instance.Level - 1, i] + "%";
            }
        }
        else
        {
            for(int i = 0; i < 5; i++)
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
        if(!ShopLocked)
        {
            Reroll();
        }
        else
        {
            ShopLocked = false;
        }
    }
}
