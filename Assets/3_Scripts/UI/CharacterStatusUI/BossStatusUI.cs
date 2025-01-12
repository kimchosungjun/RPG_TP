using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossStatusUI : EliteMonsterStatusUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    [SerializeField, Tooltip("0:Lv, 1:Name, 2:HP")] Text[] statusTexts;
    [SerializeField, Tooltip("0:HP Effect, 1:HP Fill, 2:HP Frame , 3:Groggy Effect, 4:Groggy Fill, 5:Groggy Frame")] Image[] statusImages;
    [SerializeField] GameObject statusParent;
    MonsterStat stat = null;
    /******************************************/
    /***************** Set Data **************/
    /******************************************/

    #region Set Data & Link UI 
    public override void Init()
    {
        TurnOff();
        SetImages();
    }

    public void ChangeData(MonsterStat _monsterStat)
    {
        //set hp
        stat = _monsterStat;
        float currentHP = _monsterStat.CurrentHP;
        float maxHp = _monsterStat.MaxHP;
        
        float fillValue = currentHP / maxHp;
        statusImages[0].fillAmount = fillValue;
        statusImages[1].fillAmount = fillValue;

        float groggyValue = _monsterStat.GroggyStat.CurrentGroggyValue;
        statusImages[3].fillAmount = groggyValue;
        statusImages[4].fillAmount = groggyValue;

        TurnOn();
    }
    #endregion

    /******************************************/
    /************ Update Data **************/
    /******************************************/



    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface Method
    public void TurnOn()
    {
        statusParent.SetActive(true);
    }

    public void TurnOff()
    {
        statusParent.SetActive(false);
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

}
