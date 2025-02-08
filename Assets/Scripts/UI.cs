using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public GameObject Shop, RerollButton, ShopButton;
    public int[,] RollChance = new int[11, 5] {
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
    public List<GameObject> BuyButtons;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Reroll();
    }
    public void Reroll()
    {
        foreach(GameObject Button in BuyButtons)
        {
            Button.SetActive(true);
            GameObject RandomUnit = GridGen.Instance.UnitList[Random.Range(0, GridGen.Instance.UnitList.Count)];
            Button.GetComponent<Button>().Unit = RandomUnit;
            Button.transform.GetChild(0).GetComponent<Text>().text = RandomUnit.GetComponent<Unit>().Stats.Cost.ToString();
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
        RerollButton.SetActive(true);
    }
    private void CloseShop()
    {
        Shop.SetActive(false);
        RerollButton.SetActive(false);
    }
}
