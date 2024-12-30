using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public List<GameObject> ShopButtons, BenchSlots;
    public void Buy(GameObject Champ)
    {
        Instantiate(Champ, new Vector2(Random.Range(-5, 5), Random.Range(-3, 1)), Quaternion.identity);
        gameObject.SetActive(false);
        Debug.Log("asdasd");
    }
    public void Reroll()
    {
        foreach(GameObject Button in ShopButtons)
        {
            Button.SetActive(true);
        }
    }
}
