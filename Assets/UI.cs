using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject Shop, RerollButton, ShopButton;
    public List<GameObject> BuyButtons;
    public void Reroll()
    {
        foreach(GameObject Button in BuyButtons)
        {
            Button.SetActive(true);
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
