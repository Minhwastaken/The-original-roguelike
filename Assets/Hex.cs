using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int X, Y;
    public List<int> DirX, DirY;
    public List<GameObject> AdjHexes;
    public bool Taken;
    public GameObject Reserved;
    private void Start()
    {
        GridGen.Hexes[X,Y] = gameObject; 
        /*
        for(int i = 0; i < 6; i++)
        {
            if(X + DirX[i] >= 0 && X + DirX[i] < 8 && Y + DirY[i] >= 0 && Y + DirY[i] < 8)
            {
                AdjHexes.Add(GridGen.Hexes[Y + DirY[i],X + DirX[i]]);
            }
        }
        */
    }
}
