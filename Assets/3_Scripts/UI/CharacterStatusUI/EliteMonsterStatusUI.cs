using UnityEngine;
using UnityEngine.UI;
using UIEnums;
using System.Collections;

public class EliteMonsterStatusUI : StatusUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/
    [SerializeField] protected Text hpText;
    [SerializeField, Tooltip("0:Lv, 1:Name, 2:HP")] Text[] statusTexts;
    [SerializeField, Tooltip("0:HP Effect, 1:HP Fill, 2:HP Frame , 3:Groggy Effect, 4:Groggy Fill, 5:Groggy Frame")] Image[] statusImages;
    MonsterStat stat = null;
    EliteMonster.EliteGauge gauge = null;
    /******************************************/
    /***************** Set Data **************/
    /******************************************/

    #region Set Data & Link UI 
    public override void Init()
    {
        TurnOff();
        SetImages();
    }

    public void ChangeData()
    {
        stat = null;
        gauge = null;
    }

    public void ChangeData(MonsterStat _monsterStat, EliteMonster.EliteGauge _gauge)
    {
        //set hp
        stat = _monsterStat;
        gauge = _gauge;
        float currentHP = _monsterStat.CurrentHP;
        float maxHp = _monsterStat.MaxHP;
        float fillValue = currentHP / maxHp;
        statusImages[0].fillAmount = fillValue;
        statusImages[1].fillAmount = fillValue;
        statusImages[3].fillAmount = gauge.GetGroggyGauge / 100f;
        statusImages[4].fillAmount = statusImages[3].fillAmount;

        statusTexts[0].text = "Lv."+stat.Level;
        statusTexts[1].text = stat.GetActorName;
        statusTexts[2].text = stat.CurrentHP + "/" + stat.MaxHP;
    }

    #endregion                        

    /******************************************/
    /*********** Rotate Method ************/
    /******************************************/
    public virtual void CamTransform()  {  }
    public virtual void Rotate() { }      

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface Method
    public void TurnOn()
    {
        if (gauge == null || stat == null) return;
        statusParentObject.SetActive(true);
    }

    public void TurnOff()
    {
        statusParentObject.SetActive(false);
        stat = null;
        gauge = null;
    }

    public void SetImages()
    {
        ResourceMgr resource = SharedMgr.ResourceMgr;
        statusImages[0].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Loading_Line");
        statusImages[1].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Loading_Line");
        statusImages[2].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Loading_Bar");
        statusImages[3].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Groggy_line");
        statusImages[4].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Groggy_line");
        statusImages[5].sprite = resource.GetSpriteAtlas("Bar_Atlas", "Loading_Bar");
    }
    #endregion

    protected Coroutine hpUpdateCor = null;
    public override void UpdateStatusData()
    {
        if (stat == null) return;

        statusImages[1].fillAmount = (float)stat.CurrentHP / stat.MaxHP;
        statusTexts[2].text = stat.CurrentHP + "/" + stat.MaxHP;

        if (statusImages[0].fillAmount < statusImages[1].fillAmount)
        {
            statusImages[0].fillAmount = statusImages[1].fillAmount;
            return;
        }

        if (hpUpdateCor != null)
            return;
        hpUpdateCor = StartCoroutine(CHpUpdateCor());
    }

    protected IEnumerator CHpUpdateCor()
    {
        while (true)
        {
            statusImages[0].fillAmount -= Time.deltaTime;
            if (statusImages[0].fillAmount < statusImages[1].fillAmount)
                break;
            yield return null;
        }
        statusImages[0].fillAmount = statusImages[1].fillAmount;
        hpUpdateCor = null;
    }

    private void FixedUpdate()
    {
        if (gauge == null)
            return;

        gauge.FixedGroggyDecrease();
        statusImages[4].fillAmount = gauge.GetGroggyGauge / 100f;

        // HP Effect
        if (statusImages[0].fillAmount > statusImages[1].fillAmount)
            statusImages[0].fillAmount -= Time.fixedDeltaTime;
        else if (statusImages[0].fillAmount < statusImages[1].fillAmount)
            statusImages[0].fillAmount = statusImages[1].fillAmount;

        // Gauge Effect
        if (statusImages[3].fillAmount > statusImages[4].fillAmount)
            statusImages[3].fillAmount -= Time.fixedDeltaTime;
        else if (statusImages[3].fillAmount < statusImages[4].fillAmount)
            statusImages[3].fillAmount = statusImages[4].fillAmount;
    }
}
