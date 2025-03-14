using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SegmentedHpBar : MonoBehaviour
{
    [SerializeField] private Unit Unit;
    [SerializeField] private float CUHpDelay, CUHpBarSpeed;
    public Slider CUHpBar;
    public Text HpText;
    private float CUTimer;
    public static float HpPerSegment = 5;
    public GameObject HoriLayoutGroup, Segment;
    public RectTransform FillArea;
    [SerializeField] private List<GameObject> Segments = new List<GameObject>();
    public Coroutine CatchUpCoroutine;
    private void Start()
    {
        Unit = gameObject.transform.parent.GetComponent<Unit>();
        Unit.OnTakeDamage.AddListener(UpdateHp);
        Unit.OnGridReset.AddListener(UpdateHp);
        UpdateHp();
        CUHpBar.maxValue = Unit.MaxHp;
        CUHpBar.value = Unit.Hp;
    }
    public void UpdateHp()
    {
        CUHpBar.maxValue = Unit.MaxHp;
        //HpText.text = Mathf.Round(Hp).ToString() + " / " + Mathf.Round(MaxHp).ToString();
        UpdateSegments();
        if(CatchUpCoroutine != null) 
        {
            StopCoroutine(CatchUpCoroutine);
        }
        CatchUpCoroutine = StartCoroutine(HpBarCatchUp());
    }
    [ContextMenu("Upd Segs")]
    public void UpdateSegments()
    {
        float SegmentsNum = Unit.Hp / HpPerSegment;
        while(Segments.Count < SegmentsNum)
        {
            GameObject NewSegment = Instantiate(Segment, HoriLayoutGroup.transform);
            Segments.Add(NewSegment);
        }
        float SegmentWidth = FillArea.rect.width / Unit.MaxHp * HpPerSegment;
        for(int i = 0; i < Segments.Count; i++)
        {
            if(i<SegmentsNum)
            {
                Segments[i].SetActive(true);
                Segments[i].GetComponent<RectTransform>().sizeDelta = new Vector2(SegmentWidth, Segments[i].GetComponent<RectTransform>().sizeDelta.y);
            }
            else
                Segments[i].SetActive(false);
        }
        if(SegmentsNum != 0)
        {
            if(SegmentsNum - Mathf.Floor(SegmentsNum) > 0)
            {
                Debug.Log(SegmentsNum + " 1");
                Segments[Mathf.FloorToInt(SegmentsNum)].GetComponent<RectTransform>().sizeDelta = new Vector2(SegmentWidth * (SegmentsNum - Mathf.Floor(SegmentsNum)), Segments[Mathf.FloorToInt(SegmentsNum)].GetComponent<RectTransform>().sizeDelta.y);
            }
        else
            {
                Debug.Log(SegmentsNum + " 2");
                Segments[Mathf.FloorToInt(SegmentsNum) - 1].GetComponent<RectTransform>().sizeDelta = new Vector2(SegmentWidth, Segments[Mathf.FloorToInt(SegmentsNum) - 1].GetComponent<RectTransform>().sizeDelta.y);
            }
        }
    }
    private IEnumerator HpBarCatchUp()
    {
        yield return new WaitForSeconds(CUHpDelay);
        float ChipProgression = 0;
        while(ChipProgression < 0.95f)
        {
            CUTimer += Time.deltaTime;
            ChipProgression = CUTimer * CUHpBarSpeed;
            ChipProgression *= ChipProgression;
            CUHpBar.value = Mathf.Lerp(CUHpBar.value, Unit.Hp, ChipProgression);
            yield return null;
        }
        CUHpBar.value = Unit.Hp;
        CUTimer = 0;
    }
}
