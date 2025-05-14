using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Gold, Level = 1, Xp, Stage = 1, MaxInterest = 5, RerollCost = 2;
    public List<int> XpCost = new List<int>{0, 2, 2, 6, 10, 20, 36, 48, 72, 84, 120};
    public enum GameState {Prep, Battle};
    public GameState State;
    public UnityEvent OnPrepStart, OnPrepEnd, OnBattleStart, OnBattleEnd, OnGoldGain;
    public List<TraitDefinition> TraitList;
    public Dictionary<TraitDefinition.Traits, TraitDefinition> TraitDef = new Dictionary<TraitDefinition.Traits, TraitDefinition>();
    public Dictionary<TraitDefinition.Traits, int> ActiveTraits = new Dictionary<TraitDefinition.Traits, int>();
    public List<Item> ItemList = new List<Item>();
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
        foreach(TraitDefinition Trait in TraitList)
        {
            TraitDef[Trait.TraitType] = Trait;
        }
    }

    public void AddTraits(Unit Unit)
    {
        foreach(GameObject OtherUnit in GridGen.Instance.Units)
        {
            Unit OtherUnitScript = OtherUnit.GetComponent<Unit>();
            if(OtherUnitScript.Stats == Unit.Stats && Unit != OtherUnitScript)
            {
                return;
            }
        }
        foreach(TraitDefinition.Traits Trait in Unit.Traits)
        {
            if(!ActiveTraits.ContainsKey(Trait))
            {
                ActiveTraits[Trait] = 0;
            }
            ActiveTraits[Trait]++;
            UpdateTrait(Trait);
        }
        foreach(TraitDefinition.Traits Trait in ActiveTraits.Keys)
        {
            //Debug.Log(Trait + " : " + ActiveTraits[Trait] + " activate");
        }
        UpdateTraitVisuals();
    }
    public void RemoveTraits(Unit Unit)
    {
        foreach(GameObject OtherUnit in GridGen.Instance.Units)
        {
            Unit OtherUnitScript = OtherUnit.GetComponent<Unit>();
            if(OtherUnitScript.Stats == Unit.Stats)
            {
                return;
            }
        }
        foreach(TraitDefinition.Traits Trait in Unit.Traits)
        {
            ActiveTraits[Trait]--;
            if(ActiveTraits[Trait] <= 0)
            {
                ActiveTraits.Remove(Trait);
                DeactivateTrait(Trait, TraitLevel(Trait));
            }
            else
            {
                UpdateTrait(Trait);
            }
        }
        foreach(TraitDefinition.Traits Trait in ActiveTraits.Keys)
        {
            //Debug.Log(Trait + " : " + ActiveTraits[Trait] + " deactivate");
        }
        UpdateTraitVisuals();
    }
    public void UpdateTrait(TraitDefinition.Traits Trait)
    {
        if(ActiveTraits[Trait] < TraitDef[Trait].Thresholds[0])
        {
            DeactivateTrait(Trait, TraitLevel(Trait));
            return;
        }
        ActivateTrait(Trait, TraitLevel(Trait));
    }
    public int TraitLevel(TraitDefinition.Traits Trait)
    {
        for(int level = TraitDef[Trait].Thresholds.Count - 1; level >= 0; level--)
        {
            if(ActiveTraits[Trait] >= TraitDef[Trait].Thresholds[level])
            {
                return level;
            }
        }
        Debug.Log("Trait " + Trait + " not activated");
        return -1;
    }
    public void ActivateTrait(TraitDefinition.Traits Trait, int Level)
    {
        foreach(GameObject Unit in GridGen.Instance.Units)
        {
            Unit UnitScript = Unit.GetComponent<Unit>();
            foreach(TraitDefinition.Traits UnitTrait in UnitScript.Traits)
            {
                if(UnitTrait == Trait)
                {
                    TraitDef[Trait].ApplyTrait(Unit.GetComponent<Unit>(), Level);
                    break;
                }
            }
        }
    }
    public void DeactivateTrait(TraitDefinition.Traits Trait, int Level)
    {
        if(Level < 0)
        {
            return;
        }
        foreach(GameObject Unit in GridGen.Instance.BenchedUnits)
        {
            Unit UnitScript = Unit.GetComponent<Unit>();
            foreach(TraitDefinition.Traits UnitTrait in UnitScript.Traits)
            {
                if(UnitTrait == Trait)
                {
                    TraitDef[Trait].UnapplyTrait(Unit.GetComponent<Unit>(), Level);
                    break;
                }
            }
        }
        foreach(GameObject Unit in GridGen.Instance.Units)
        {
            Unit UnitScript = Unit.GetComponent<Unit>();
            foreach(TraitDefinition.Traits UnitTrait in UnitScript.Traits)
            {
                if(UnitTrait == Trait)
                {
                    TraitDef[Trait].UnapplyTrait(Unit.GetComponent<Unit>(), Level);
                    break;
                }
            }
        }
    }
    public void UpdateTraitVisuals()
    {
        List<TraitDefinition.Traits> Traits = ActiveTraits.Keys.OrderByDescending(trait => ActiveTraits[trait]).ThenBy(trait => TraitDef[trait].Thresholds[^1]).ToList();
        foreach(TraitDefinition.Traits Trait in Traits)
        {
            //Debug.Log("sort: " + Trait + " : " + ActiveTraits[Trait]);
        }
        for(int order = 0; order < UI.Instance.TraitHolder.transform.childCount; order++)
        {
            if(order >= Traits.Count)
            {
                UI.Instance.TraitHolder.transform.GetChild(order).gameObject.SetActive(false);
                continue;
            }
            else
            {
                UI.Instance.TraitHolder.transform.GetChild(order).gameObject.SetActive(true);
            }
            TraitVisual Visual = UI.Instance.TraitHolder.transform.GetChild(order).GetComponent<TraitVisual>();
            Visual.Name.text = TraitDef[Traits[order]].Name;
            Visual.Count.text = ActiveTraits[Traits[order]].ToString();
        }
    }
    public void UpdateItemVisuals()
    {
        for(int order = 0; order < UI.Instance.ItemHolder.transform.childCount; order++)
        {
            if(order >= ItemList.Count)
            {
                UI.Instance.ItemHolder.transform.GetChild(order).gameObject.SetActive(false);
                continue;
            }
            else
            {
                UI.Instance.ItemHolder.transform.GetChild(order).gameObject.SetActive(true);
            }
            Image Visual = UI.Instance.ItemHolder.transform.GetChild(order).GetComponent<Image>();
            Visual.sprite = ItemList[order].GetComponent<SpriteRenderer>().sprite;
        }
    }
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
    }
    public void GainXp(int XpGain)
    {
        Xp += XpGain;
        if(Level > 10)
        {
            return;
        }
        if(Xp >= XpCost[Level])
        {
            Xp -= XpCost[Level];
            Level++;
            UI.Instance.UpdateRollChance();
        }
        UI.Instance.XpText.text= Xp + "/" + XpCost[Level];
    }
}
