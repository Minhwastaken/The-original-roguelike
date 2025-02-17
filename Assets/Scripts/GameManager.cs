using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Gold, Level = 1, Xp, Stage = 1, MaxInterest = 5;
    public List<int> XpCost = new List<int>{0, 2, 2, 6, 10, 20, 36, 48, 72, 84, 120};
    public enum GameState {Prep, Battle};
    public GameState State;
    public UnityEvent OnPrepStart, OnPrepEnd, OnBattleStart, OnBattleEnd, OnGoldGain;
    // Start is called before the first frame update
    private void Awake() 
    {
        Instance = this;
    }
    void Start()
    {
        State = GameState.Prep;
        OnPrepStart.AddListener(GainGoldWithInterest);
        OnPrepStart.AddListener(() => GainXp(2));
    }

    // Update is called once per frame
    public void GainGoldWithInterest()
    {
        Gold += 5 + Mathf.Min(Gold/10, MaxInterest);
        UI.Instance.GoldText.text = Gold + "G";
    }
    public void GainGold(int Amount)
    {
        Gold += Amount;
        UI.Instance.GoldText.text = Gold + "G";
        OnGoldGain.Invoke();
    }
    public void LoseGold(int amount)
    {
        Gold -= amount;
        UI.Instance.GoldText.text = Gold + "G";
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
            OnPrepEnd.Invoke();
            OnBattleStart.Invoke();
        }
        else
        {
            Stage++;
            UI.Instance.StageText.text = "Stage " + Stage;
            State = GameState.Prep;
            OnBattleEnd.Invoke();
            OnPrepStart.Invoke();
        }
        Debug.Log("It's currently in " + State + " state");
    }
    public void BuyXp()
    {
        if(Gold >= 4)
        {
            GainXp(4);
            LoseGold(4);
        }
        Debug.Log(Level);
    }
    public void GainXp(int XpGain)
    {
        Xp += XpGain;
        if(Xp >= XpCost[Level])
        {
            Xp -= XpCost[Level];
            Level++;
        }
        UI.Instance.XpText.text = Xp + "/" + XpCost[Level] + "XP";
    }
}
