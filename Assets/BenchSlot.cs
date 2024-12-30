using UnityEngine;

public class BenchSlot : MonoBehaviour
{
    private void Awake() 
    {
        char id = gameObject.name[^2];
        Debug.Log(gameObject.name[^2]);
        if(id - '0' > 0 && id - '0' < 9)
        {
            GridGen.BenchSlots[id - '0'] = gameObject;
        }
        else
        {
            GridGen.BenchSlots[0] = gameObject;
        }
    }
}
