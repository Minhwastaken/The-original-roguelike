using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBarManager : MonoBehaviour
{
    [SerializeField] private Unit Unit;
    [SerializeField] private float CUHPDelay, CUHPBarSpeed;
    public Slider HPBar, CUHPBar;
    public Text HPText;
    [SerializeField] private Image HPBarImage, CUHPBarImage;
    public float CUHPDelayTimer, CUTimer;
    private Coroutine CatchUpCoroutine;
    private void Start()
    {
        Unit = gameObject.transform.parent.GetComponent<Unit>();
        Unit.OnTakeDamage.AddListener(UpdateHP);
        Unit.OnGridReset.AddListener(UpdateHP);
        HPBar.maxValue = Unit.MaxHp;
        HPBar.value = Unit.Hp;
        CUHPBar.maxValue = Unit.MaxHp;
        CUHPBar.value = Unit.Hp;
    }
    public void UpdateHP()
    {
        HPBar.maxValue = Unit.MaxHp;
        CUHPBar.maxValue = Unit.MaxHp;
        HPBar.value = Unit.Hp;
        //HPText.text = Mathf.Round(Hp).ToString() + " / " + Mathf.Round(MaxHp).ToString();
        if(CatchUpCoroutine != null) 
        {
            StopCoroutine(CatchUpCoroutine);
        }
        CatchUpCoroutine = StartCoroutine(HpBarCatchUp());
    }
    private IEnumerator HpBarCatchUp()
    {
        float CurrentHp = Unit.Hp;
        while(CUHPDelayTimer < CUHPDelay && CurrentHp == Unit.Hp)
        {
            CUHPDelayTimer += Time.deltaTime;
            yield return null;
        }
        if(CUHPDelayTimer >= CUHPDelay)
        {
            float ChipProgression = 0;
            while(ChipProgression < 0.95f && CurrentHp == Unit.Hp)
            {
                CUTimer += Time.deltaTime;
                ChipProgression = CUTimer * CUHPBarSpeed;
                ChipProgression *= ChipProgression;
                CUHPBar.value = Mathf.Lerp(CUHPBar.value, HPBar.value, ChipProgression);
                yield return null;
            }
            if(CurrentHp == Unit.Hp)
            {
                CUHPBar.value = HPBar.value;
                CUTimer = 0;
                CUHPDelayTimer = 0;
            }
        }
    }
}
