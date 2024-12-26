using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GridGen : MonoBehaviour
{
    public int GridLength, GridHeight;
    public float HexLength, HexHeight;
    public GameObject Hex, Grid;
    [SerializeField] public static GameObject[,] Hexes = new GameObject[1000, 1000];
    public static List<GameObject> Units = new List<GameObject>(), Enemies = new List<GameObject>();
    public Color ColorOne, ColorTwo;
    
    private float WorldGridHeight, WorldGridLength;
    [HideInInspector] public InputField LengthIP, CrossIP, HeightIP;
    private void Start()
    {
        HexLength = Hex.transform.localScale.x / 1.15f;
        HexHeight = HexLength * Mathf.Cos(30 * Mathf.Deg2Rad);
        WorldGridHeight = GridHeight * HexHeight;
        WorldGridLength = GridLength * HexLength;
        //GenerateGrid();
        //AddAdjHexes();
    }
    [ContextMenu("Generate Hex Grid")]
    public void GenerateGrid()
    {
        if(LengthIP && LengthIP.text.Length > 0)
            GridLength = int.Parse(LengthIP.text);
        if(HeightIP && HeightIP.text.Length > 0)
            GridHeight = int.Parse(HeightIP.text);
            
        foreach (GameObject SpawnedHex in Hexes)
        {
            DestroyImmediate(SpawnedHex);
        }
        //Camera.main.gameObject.transform.position = new Vector3(GridLength / 5f, GridHeight / 2.225f, -10);
        Camera.main.gameObject.transform.position = new Vector3(WorldGridLength / 2f, WorldGridHeight / 2.225f, -10);
        for (int i = 0; i < GridHeight; i++)
        {
            float EvenOffset = 0;
            List<int> DirX = new List<int>{-1, -1, 0, 1, 0, -1}, DirY = new List<int>{0, 1, 1, 0, -1, -1};
            if(i%2==0)
            {
                EvenOffset = 0.5f * HexLength;
                DirX = new List<int>{-1, 0, 1, 1, 1, 0};
                DirY = new List<int>{0, 1, 1, 0, -1, -1};
            }
            for (int y = 0; y < GridLength; y++)
            {
                GameObject SpawnedHex = Instantiate(Hex, new Vector3(y * HexLength + EvenOffset, i * HexHeight, 10), Quaternion.identity);
                SpawnedHex.transform.parent = Grid.transform;
                Hexes[i, y] = SpawnedHex;
                SpawnedHex.GetComponent<Hex>().X = y;
                SpawnedHex.GetComponent<Hex>().Y = i;
                SpawnedHex.GetComponent<Hex>().DirX = DirX;
                SpawnedHex.GetComponent<Hex>().DirY = DirY;
                SpawnedHex.name = "Hex " + y + " " + i;
                /*
                if(AltSpawn)
                {
                    if((i + y) % 2 == 0)
                        SpawnedHex.GetComponent<SpriteRenderer>().color = ColorOne;
                    else
                        SpawnedHex.GetComponent<SpriteRenderer>().color = ColorTwo;
                }
                */
            }
        }
        AddAdjHexes();
    }

    public void AddAdjHexes()
    {
        for(int i = 0; i < GridLength; i++)
        {
            for(int y = 0; y < GridHeight; y++)
            {
                Hex TempHex = Hexes[i,y].GetComponent<Hex>();
                for(int z = 0; z < 6; z++)
                {
                    if(TempHex.X + TempHex.DirX[z] >= 0 && TempHex.X + TempHex.DirX[z] < 8 && TempHex.Y + TempHex.DirY[z] >= 0 && TempHex.Y + TempHex.DirY[z] < 8)
                    {
                        TempHex.AdjHexes.Add(Hexes[TempHex.Y + TempHex.DirY[z],TempHex.X + TempHex.DirX[z]]);
                    }
                }  
            }
        }        
    }

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        foreach (GameObject SpawnedHex in Hexes)
        {
            DestroyImmediate(SpawnedHex);
        }
    }
}
