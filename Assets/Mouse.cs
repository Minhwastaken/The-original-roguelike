using System;
using System.Collections;
using System.Collections.Generic;

//using System.Numerics;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private Camera Cam;
    private Vector2 OriMousePos, OriHex = new Vector2(-1,-1);
    public GameObject ChosenObject;
    public float DragThres = 0.1f;
    public bool IsFollowing, IsDragging;
    private readonly int UnitLayer = 3, HexLayer = 6;
    void Start()
    {
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
                        OriMousePos = Cam.ScreenToWorldPoint(Input.mousePosition);
                        Debug.Log("Chosen GameObject: " + Hit.transform.gameObject.name);
                    }
                }
                else if(Hit.transform.gameObject.layer == HexLayer)
                {
                    Hit.transform.GetComponent<Hex>().Taken = !Hit.transform.GetComponent<Hex>().Taken;
                    if(Hit.transform.GetComponent<Hex>().Taken)
                    {
                        Hit.transform.GetComponent<SpriteRenderer>().color = Color.blue;
                    }
                    else
                    {
                        Hit.transform.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
            else
            {
                Debug.Log("You clicked nothing!");
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
                 //add stuff that happens on release
                }
                else if(IsDragging == false || IsFollowing == false)
                {
                    IsFollowing = true;
                }
                /*else if(IsFollowing)
                {
                    DropUnit();
                    //add stuff that happens on release
                }*/
            }
            
        }
    }

    private void HexLockIn()
    {
        float MinDist = 1;
        GameObject NearestHex = null;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                //Debug.Log(GridGen.Hexes[i,j]);
                if(GridGen.Hexes[i,j])
                {
                    float Dist = Vector2.Distance(ChosenObject.transform.position, GridGen.Hexes[i,j].transform.position);
                     if(Dist < 0.5f && Dist < MinDist)
                    {
                        MinDist = Dist;
                        NearestHex = GridGen.Hexes[i,j];
                    }
                }
            }
        }
        if(NearestHex)
        {
            ChosenObject.transform.position = (Vector2) NearestHex.transform.position;
            OriHex = (Vector2) NearestHex.transform.position;
            ChosenObject.GetComponent<Unit>().Hex = NearestHex;
        }
        else if(OriHex != new Vector2(-1,-1))
        {
            ChosenObject.transform.position = OriHex;
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
}
