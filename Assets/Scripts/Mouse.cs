using System;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public static Mouse Instance;
    private Camera Cam;
    private Vector2 OriMousePos, OriHex = new Vector2(-1,-1);
    public GameObject ChosenObject;
    public float DragThres = 0.1f, SnapDist;
    public bool IsFollowing, IsDragging;
    private readonly int UnitLayer = 3;
    // HexLayer = 6;
    void Start()
    {
        Instance = this;
        Cam = Camera.main;
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D Hit = Physics2D.Raycast(Cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(Hit)
            {
                if(Hit.transform.gameObject.layer == UnitLayer)
                {
                    if(Hit.transform.gameObject == ChosenObject)
                    {
                        DropUnit();
                        //add stuff that happens on release
                    }
                    else
                    {
                        ChosenObject = Hit.transform.gameObject;
                        if(ChosenObject.GetComponent<Unit>().Hex)
                        {
                            OriHex = ChosenObject.GetComponent<Unit>().Hex.transform.position;
                        }
                        OriMousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
                        //Debug.Log("Chosen GameObject: " + Hit.transform.gameObject.name);
                    }
                }
                /*
                else if(Hit.transform.gameObject.layer == HexLayer)
                {
                }*/
            }
            else
            {
                //Debug.Log("You clicked nothing!");
            }
        }

        if(Input.GetMouseButton(0))
        {
            if(ChosenObject)
            {
                if(ChosenObject.layer == UnitLayer)
                {
                    ChosenObject.transform.position = (Vector2) Cam.ScreenToWorldPoint(Input.mousePosition);
                    if(Vector2.Distance((Vector2)Cam.ScreenToWorldPoint(Input.mousePosition), OriMousePos) > DragThres)
                    {
                        IsDragging = true;
                    }
                }
            }
        }

        if(IsFollowing)
        {
            if(ChosenObject)
            {
                ChosenObject.transform.position = (Vector2) Cam.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(ChosenObject)
            {
                if(IsDragging)
                {
                    DropUnit();
                }
                else if(IsDragging == false || IsFollowing == false)
                {
                    IsFollowing = true;
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit2D Hit = Physics2D.Raycast(Cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(Hit)
            {
                if(Hit.transform.gameObject.layer == UnitLayer)
                {
                    Unit HitUnit = Hit.transform.gameObject.GetComponent<Unit>();
                    Debug.Log("Unit: " + HitUnit.name);
                    foreach(Buff buff in HitUnit.Buffs)
                    {
                        Debug.Log("Buff: " + buff.Name);
                    }
                }
            }
        }
    }

    private void HexLockIn()
    {
        float MinDist = 1;
        GameObject Nearest = null;
        Unit UnitScript = ChosenObject.GetComponent<Unit>();

        //if unit cap is not yet reached, or unit is alr on field, then check the grid
        if(GridGen.Instance.Units.Count < GameManager.Instance.Level || ChosenObject.GetComponent<Unit>().Hex.GetComponent<Hex>() || ChosenObject.GetComponent<Unit>().Team != 0)
        {
            for(int i = 0; i < GridGen.Instance.GridLength; i++)
            {
                for(int j = 0; j < GridGen.Instance.GridHeight; j++)
                {
                    if(!GridGen.Instance.Hexes[i,j].GetComponent<Hex>().Taken)
                    {
                        float Dist = Vector2.Distance(ChosenObject.transform.position, GridGen.Instance.Hexes[i,j].transform.position);
                        if(Dist < SnapDist && Dist < MinDist)
                        {
                            MinDist = Dist;
                            Nearest = GridGen.Instance.Hexes[i,j];
                        }
                    }
                }
            }
        }
        //Debug.Log("Nearest Hex: " + NearestHex.name);
        if(Nearest)
        {
            Debug.Log("Nearest Hex: " + Nearest.name);
            ResetOGHex(UnitScript);
            ChosenObject.transform.position = (Vector2) Nearest.transform.position;
            OriHex = (Vector2) Nearest.transform.position;
            UnitScript.Hex = Nearest;
            UnitScript.OGHex = Nearest;
            Nearest.GetComponent<Hex>().Taken = ChosenObject;
            if(UnitScript.Benched)
            {
                UnitScript.Benched = false;
                GridGen.Instance.BenchedUnits.Remove(ChosenObject);
                GridGen.Instance.Units.Add(ChosenObject);
            }
        }
        else
        {
            foreach(GameObject BenchSlot in GridGen.Instance.BenchSlots)
            {
                if(!BenchSlot.GetComponent<BenchSlot>().Taken)
                {
                    float Dist = Vector2.Distance(ChosenObject.transform.position, BenchSlot.transform.position);
                    if(Dist < SnapDist && Dist < MinDist)
                    {
                        MinDist = Dist;
                        Nearest = BenchSlot;
                    }
                }
            }
            if(Nearest)
            {
                ResetOGHex(UnitScript);
                ChosenObject.transform.position = (Vector2) Nearest.transform.position;
                UnitScript.Hex = Nearest;
                UnitScript.OGHex = Nearest;
                Nearest.GetComponent<BenchSlot>().Taken = ChosenObject;
                if(UnitScript.Benched == false)
                {
                    UnitScript.Benched = true;
                    GridGen.Instance.BenchedUnits.Add(ChosenObject);
                    GridGen.Instance.Units.Remove(ChosenObject);
                }
            }
            else if(OriHex != new Vector2(-1,-1))
            {
                ChosenObject.transform.position = OriHex;
            }
        }
    }

    private void Drop()
    {
        IsFollowing = false;
        IsDragging = false;
        ChosenObject = null;
        OriMousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
    }
    
    private void DropUnit()
    {
        if(ChosenObject.layer == UnitLayer)
        {
            HexLockIn();
        }
        Drop();
    }

    public void ResetOGHex(Unit UnitScript)
    {
        if(UnitScript.Hex)
        {
            if(UnitScript.Hex.GetComponent<Hex>())
            {
                Debug.Log("Resetting Hex");
                UnitScript.Hex.GetComponent<Hex>().Taken = null;
            }
            else if(UnitScript.Hex.GetComponent<BenchSlot>())
            {
                Debug.Log("Resetting BenchSlot");
                UnitScript.Hex.GetComponent<BenchSlot>().Taken = null;
            }
        }
    }

    private void Field(GameObject Unit)
    {
        if(Unit.GetComponent<Unit>().Team == 0)
        {
            GridGen.Instance.Units.Add(Unit);
        }
        else
        {
            GridGen.Instance.Enemies.Add(Unit);
        }
    }
    public void RemoveUnit(GameObject Unit)
    {
        if(Unit.GetComponent<Unit>().Team == 0)
        {
            if(Unit.GetComponent<Unit>().Benched)
            {
                GridGen.Instance.BenchedUnits.Remove(Unit);
            }
            else
            {
                GridGen.Instance.Units.Remove(Unit);
            }
            ResetOGHex(Unit.GetComponent<Unit>());
            /*
            if(GridGen.Instance.Units.Count == 0)
            {
                GameManager.Instance.ChangeState();
            }*/
        }
        else
        {
            if(Unit.GetComponent<Unit>().Benched)
            {
                GridGen.Instance.BenchedUnits.Remove(Unit);
            }
            else
            {
                GridGen.Instance.Units.Remove(Unit);
            }
            ResetOGHex(Unit.GetComponent<Unit>());
            /*
            if(GridGen.Instance.Enemies.Count == 0)
            {
                GameManager.Instance.ChangeState();
            }*/
        }
        Destroy(Unit);
    }
}
