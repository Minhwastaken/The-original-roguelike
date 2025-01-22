using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameState {Prep, Battle};
    public GameState State;
    public UnityEvent OnPrepStart, OnPrepEnd, OnBattleStart, OnBattleEnd;
    // Start is called before the first frame update
    private void Awake() 
    {
        Instance = this;
    }
    void Start()
    {
        State = GameState.Prep;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetState(GameState state)
    {
        State = state;
    }
    public void ChangeState()
    {
        if(State == GameState.Prep)
        {
            State = GameState.Battle;
            OnBattleStart.Invoke();
        }
        else
        {
            State = GameState.Prep;
            OnPrepStart.Invoke();
        }
        Debug.Log("It's currently in " + State + " state");
    }
}
