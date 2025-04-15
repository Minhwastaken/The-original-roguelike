using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarManager : MonoBehaviour
{
    [SerializeField] private Unit Unit;
    [SerializeField] private float CUHpDelay, CUHpBarSpeed;
    public Slider HPBar, CUHpBar;
    public Text HPText;
    [SerializeField] private Image HPFill, CUHpFill;
    public float CUTimer;
    public GameObject Bar, BarHLG;
    public List<GameObject> Bars = new List<GameObject>();
    public static float HpPerSegment = 5;
    private Coroutine CatchUpCoroutine;
    private void Start()
    {
        Unit = gameObject.transform.parent.GetComponent<Unit>();
        if(Unit.Team == 0)
        {
            HPFill.color = Color.green;
        }
        else
        {
            HPFill.color = Color.red;
        }
        Unit.OnTakeDamage.AddListener(UpdateHp);
        Unit.OnReset.AddListener(UpdateHp);
        UpdateHp();
        HPBar.maxValue = Unit.MaxHp;
        HPBar.value = Unit.Hp;
        CUHpBar.maxValue = Unit.MaxHp;
        CUHpBar.value = Unit.Hp;
    }
    public void UpdateHp()
    {
        HPBar.maxValue = Unit.MaxHp;
        CUHpBar.maxValue = Unit.MaxHp;
        HPBar.value = Unit.Hp;
        UpdateBars();
        //HPText.text = Mathf.Round(Hp).ToString() + " / " + Mathf.Round(MaxHp).ToString();
        if(CatchUpCoroutine != null) 
        {
            StopCoroutine(CatchUpCoroutine);
        }
        CatchUpCoroutine = StartCoroutine(HpBarCatchUp());
    }
    public void UpdateBars()
    {
        float SegmentNum = Unit.MaxHp / HpPerSegment, SegmentsLeft = Unit.Hp / HpPerSegment;
        float BarNum = SegmentNum - 1, BarsLeft = SegmentsLeft - 1;
        while(Bars.Count < BarNum)
        {
            GameObject NewSegment = Instantiate(Bar, BarHLG.transform);
            Bars.Add(NewSegment);
        }
        float SegmentWidth = BarHLG.GetComponent<RectTransform>().rect.width / SegmentNum;
        for(int i = 0; i < Bars.Count; i++)
        {
            if(i<BarNum)
            {
                Bars[i].transform.GetChild(0).gameObject.SetActive(true);
                Bars[i].GetComponent<RectTransform>().sizeDelta = new Vector2(SegmentWidth, Bars[i].GetComponent<RectTransform>().sizeDelta.y);
                if(i < BarsLeft)
                {
                    Bars[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                }
                else
                {
                    Bars[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                }
            }
            else
                Bars[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    private IEnumerator HpBarCatchUp()
    {
        yield return new WaitForSeconds(CUHpDelay);
        Image Fill = transform.Find("CatchUpHpBar/Fill").GetComponent<Image>();
        if(Unit.Hp > CUHpBar.value)
        {
            CUHpBar.value = Unit.Hp;
        }
        else
        {
            Fill.color = Color.yellow;
            float ChipProgression = 0, CUTimer = 0;
            while(ChipProgression < 0.95f)
            {
                CUTimer += Time.deltaTime;
                ChipProgression = CUTimer * CUHpBarSpeed;
                ChipProgression *= ChipProgression;
                CUHpBar.value = Mathf.Lerp(CUHpBar.value, Unit.Hp, ChipProgression);
                yield return null;
            }
            CUHpBar.value = Unit.Hp;
        }
    }
}
