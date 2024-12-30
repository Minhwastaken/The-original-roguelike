using UnityEngine;

public class Button : MonoBehaviour
{
      public void Buy(GameObject Unit)
    {
        Instantiate(Unit, new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 1f)), Quaternion.identity);
        gameObject.SetActive(false);
    }
}
