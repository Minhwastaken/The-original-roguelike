using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridGen : MonoBehaviour
{
    public static GridGen Instance;
    public int GridLength, GridHeight;
    public float HexLength, HexHeight;
    public GameObject Hex, Grid;
    public GameObject[,] Hexes = new GameObject[1000, 1000];
    public GameObject[] BenchSlots = new GameObject[9];
    public List<GameObject> UnitList = new List<GameObject>(), EnemyList = new List<GameObject>(),
    OneCosts = new List<GameObject>(), TwoCosts = new List<GameObject>(), ThreeCosts = new List<GameObject>(),
    FourCosts = new List<GameObject>(), FiveCosts = new List<GameObject>(),
    Units = new List<GameObject>(), Enemies = new List<GameObject>(),
    BenchedUnits = new List<GameObject>(), DeadUnits = new List<GameObject>();
    public List<List<(GameObject Unit, int Count)>> UnitPool = new List<List<(GameObject Unit, int Count)>>();
    public int OneCostCount, TwoCostCount, ThreeCostCount, FourCostCount, FiveCostCount;
    public Color ColorOne, ColorTwo;
    
    private float WorldGridHeight, WorldGridLength;
    public List<GameObject> Attacks = new List<GameObject>();
    [HideInInspector] public InputField LengthIP, CrossIP, HeightIP;
    [HideInInspector] public readonly int UnitLayer = 3, HexLayer = 6;
    private void Awake()
    {
        Instance = this;
        SortUnitsByCost();
    }
    private void Start()
    {
        HexLength = Hex.transform.localScale.x / 1.15f;
        HexHeight = HexLength * Mathf.Cos(30 * Mathf.Deg2Rad);
        WorldGridHeight = GridHeight * HexHeight;
        WorldGridLength = GridLength * HexLength;

        GameManager.Instance.OnBattleEnd.AddListener(ResetGrid);
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
                Hex TempHex = Hexes[y,i].GetComponent<Hex>();
                Debug.Log(TempHex);
                for(int z = 0; z < 6; z++)
                {
                    if(TempHex.X + TempHex.DirX[z] >= 0 && TempHex.X + TempHex.DirX[z] < GridLength && TempHex.Y + TempHex.DirY[z] >= 0 && TempHex.Y + TempHex.DirY[z] < GridHeight)
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
    [ContextMenu("Sort Units by cost")]
    public void SortUnitsByCost()
    {
        OneCosts.Clear();
        TwoCosts.Clear();
        ThreeCosts.Clear();
        FourCosts.Clear();
        FiveCosts.Clear();
        for(int i = 0; i < 5; i++)
        {
            UnitPool.Add(new List<(GameObject Unit, int Count)>());
        }
        foreach(GameObject Unit in UnitList)
        {
            Unit UnitScript = Unit.GetComponent<Unit>();
            if(UnitScript.Stats.Cost == 1)
            {
                OneCosts.Add(Unit);
                UnitPool[0].Add((Unit, OneCostCount));
            }
            else if(UnitScript.Stats.Cost == 2)
            {
                TwoCosts.Add(Unit);
                UnitPool[1].Add((Unit, TwoCostCount));
            }
            else if(UnitScript.Stats.Cost == 3)
            {
                ThreeCosts.Add(Unit);
                UnitPool[2].Add((Unit, ThreeCostCount));
            }
            else if(UnitScript.Stats.Cost == 4)
            {
                FourCosts.Add(Unit);
                UnitPool[3].Add((Unit, FourCostCount));
            }
            else if(UnitScript.Stats.Cost == 5)
            {
                FiveCosts.Add(Unit);
                UnitPool[4].Add((Unit, FiveCostCount));
            }
        }
    }
    public void ResetGrid()
    {
        foreach(GameObject Unit in DeadUnits)
        {
            Unit.SetActive(true);
            Units.Add(Unit);
        }
        DeadUnits.Clear();
        ClearAttacks();

        foreach(GameObject Unit in Units)
        {
            Unit UnitScript = Unit.GetComponent<Unit>();
            UnitScript.GetStats();
            UnitScript.UpdateBaseStats();
            UnitScript.ClearBuffs();
            UnitScript.Hex.GetComponent<Hex>().Taken = null;
            UnitScript.Hex = UnitScript.OGHex;
            UnitScript.IsMoving = false;
            UnitScript.LerpProgress = 0;
            UnitScript.Path = (null, null);
            Unit.transform.position = (Vector2) UnitScript.Hex.transform.position;
            UnitScript.Sprites.transform.rotation = Quaternion.identity;
            Unit.GetComponent<Animator>().Play("Idle");
            UnitScript.OnGridReset.Invoke();
        }
        if(GameManager.Instance.Stage >= 28)
        {
            SpawnEnemies(28);
        }
        else
            SpawnEnemies(GameManager.Instance.Stage);
    }

    public IEnumerator ClearAttacks()
    {
        yield return new WaitForSeconds(1f);
        foreach(GameObject Attack in Attacks)
        {
            Destroy(Attack);
        }
        Attacks.Clear();
    }
    public void SpawnEnemies(float Credit)
    {
        for(int i = 0; i < Credit; i++)
        {
            if(Enemies.Count >= 28)
                return;
            GameObject Enemy = Instantiate(UnitList[Random.Range(0, UnitList.Count)]);
            UnitStats.UnitRole EnemyRole = Enemy.GetComponent<Unit>().Stats.Role;
            if(EnemyRole == UnitStats.UnitRole.AttackCarry || EnemyRole == UnitStats.UnitRole.MagicCarry)
            {
                SpawnEnemy(Enemy, 3, 4);
            }
            else if(EnemyRole == UnitStats.UnitRole.AttackCaster || EnemyRole == UnitStats.UnitRole.MagicCaster)
            {
                SpawnEnemy(Enemy, 3, 4);
            }
            else if(EnemyRole == UnitStats.UnitRole.AttackTank || EnemyRole == UnitStats.UnitRole.MagicTank)
            {
                SpawnEnemy(Enemy, 1, 1);
            }
            else if(EnemyRole == UnitStats.UnitRole.AttackFighter || EnemyRole == UnitStats.UnitRole.MagicFighter)
            {
                SpawnEnemy(Enemy, 1, 3);
            }
            else if(EnemyRole == UnitStats.UnitRole.AttackReaper || EnemyRole == UnitStats.UnitRole.MagicReaper)
            {
                SpawnEnemy(Enemy, 2, 3);
            }
        }
    }
    public void SpawnEnemy(GameObject Enemy, int FrontRow, int BackRow)
    {
        List<GameObject> BacklineHexes = AvailableRows(FrontRow, BackRow);
        if(BacklineHexes.Count == 0)
        {
            if(BackRow < 4)
                SpawnEnemy(Enemy, FrontRow, BackRow + 1);
            else if(FrontRow > 1)
                SpawnEnemy(Enemy, FrontRow - 1, BackRow);
            return;
        }
        GameObject RandomHex = BacklineHexes[Random.Range(0, BacklineHexes.Count)];
        PlaceEnemy(Enemy, RandomHex);
    }
    public List<GameObject> AvailableRows(int FrontRow, int BackRow)
    {
        List<GameObject> AvailableTiles = new List<GameObject>();
        for(int i = 0; i < GridLength; i++)
        {
            for(int y = FrontRow + 3; y < BackRow + 4; y++)
            {
                Debug.Log(Hexes[i,y]);
                if(Hexes[i,y].GetComponent<Hex>().Taken == null)
                {
                    AvailableTiles.Add(Hexes[i,y]);
                }
            }
        }
        return AvailableTiles;
    }
    public void PlaceEnemy(GameObject Enemy, GameObject Hex)
    {
        Enemy.GetComponent<Unit>().Team = 1;
        Enemy.GetComponent<Unit>().Hex = Hex;
        Hex.GetComponent<Hex>().Taken = Enemy;
        Enemy.transform.position = (Vector2) Enemy.GetComponent<Unit>().Hex.transform.position;
        Enemies.Add(Enemy);
    }
}
