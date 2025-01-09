using UnityEngine;
using UnityEngine.UI;
using UIEnums;
using System.Collections;

public class PlayerStatusUI : StatusUI, ICommonSetUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [SerializeField, Header("HP Bar"), Tooltip("0:Frame, 1:Fill, 2:Effect")] Image[] hpImages;
    [SerializeField, Header("Exp Bar"), Tooltip("0:Frame, 1:Fill, 2:Effect")] Image[] expImages;


    float hpTarget = -1;
    float expTarget = -1;
    PlayerStat stat = null;
    float maxExp = -1f;
    #endregion

    /******************************************/
    /***************** Set Data **************/
    /******************************************/

    #region Set Data & Link UI 
    public override void Init()
    {
        if (playerStatusParentObject.activeSelf) playerStatusParentObject.SetActive(false);
        SetImages();
    }

    public void ChangeData(PlayerStat _playerStat)
    {
        if (isActive == false)
            return;
        // update bool init
        updateExp = false;
        updateHp = false;
        //set hp
        stat = _playerStat;
        float currentHP = _playerStat.GetSaveStat.currentHP;
        float maxHp = _playerStat.MaxHP;
        if (currentHP <= 0) currentHP = 0;
        hpImages[1].fillAmount = currentHP / maxHp;
        hpImages[2].fillAmount = hpImages[1].fillAmount;
        // set exp
        maxExp = SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().needExps[_playerStat.GetSaveStat.currentLevel - 1];
        if (maxExp < 0f)
            expImages[1].fillAmount = 1f;
        else
        {
            expImages[1].fillAmount = _playerStat.GetSaveStat.currentExp / maxExp;
            expImages[2].fillAmount = expImages[1].fillAmount;
        }
        // set text
        hpText.text = (int)currentHP + "/" + (int)maxHp;
        levelText.text = "Lv." + _playerStat.GetSaveStat.currentLevel;
        // turn on
        if (playerStatusParentObject.activeSelf == false) playerStatusParentObject.SetActive(true);
    }
    #endregion

    /******************************************/
    /************ Update Data **************/
    /******************************************/

    #region Update UI

    public void UpdateData(UIEnums.STATUS _updateStatusType)
    {
        switch (_updateStatusType)
        {
            case STATUS.HP:
                UpdateHP();
                break;
            case STATUS.EXP:
                UpdateExp();
                break;
            case STATUS.LEVEL:
                UpdateLevel();
                break;
        }
    }

    bool updateHp = false;
    public void UpdateHP()
    {
        hpTarget = stat.GetSaveStat.currentHP;
        hpText.text = hpTarget + "/" + stat.MaxHP;  
        if (updateHp == false)
        {
            updateHp = true;    
            StartCoroutine(CUpdateHP());
        }
    }

    IEnumerator CUpdateHP()
    {
        hpImages[1].fillAmount = (float)stat.GetSaveStat.currentHP / stat.MaxHP;
        while (true)
        {
            if (hpImages[2].fillAmount <= hpImages[1].fillAmount)
            {
                updateHp = false;
                break;
            }
            hpImages[2].fillAmount -= Time.fixedDeltaTime/effectTime;
            yield return new WaitForFixedUpdate();
        }   
    }

    bool updateExp = false;
    public void UpdateExp()
    {
        expTarget = stat.GetSaveStat.currentExp;
        if (updateExp == false)
        {
            updateExp = true;
            StartCoroutine(CUpdateExp());
        }
    }
    IEnumerator CUpdateExp()
    {
        maxExp = SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().needExps[stat.GetSaveStat.currentLevel - 1];
        
        if(maxExp < 0)
        {
            expImages[1].fillAmount = 1;
            expImages[2].fillAmount = 1;
            yield break;
        }
        while (true)
        {
            if (expImages[2].fillAmount <= expImages[1].fillAmount)
            {
                updateExp = false;
                break;
            }
            expImages[2].fillAmount -= Time.fixedDeltaTime / effectTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public void UpdateLevel()
    {
        levelText.text = "Lv."+stat.GetSaveStat.currentLevel;
    }
    #endregion

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface Method
    bool isActive = true;
    public bool IsActive()
    {
        return isActive;
    }

    public void Active()
    {
        isActive = true;
        playerStatusParentObject.SetActive(true);
    }

    public void InActive()
    {
        isActive = false;
        playerStatusParentObject.SetActive(false);
    }

    public void SetImages()
    {
        ResourceMgr resource = SharedMgr.ResourceMgr;
        hpImages[0].sprite = resource.GetSpriteAtlas("Bar_Atlas", "HP_Bar");
        hpImages[1].sprite = resource.GetSpriteAtlas("Bar_Atlas", "HP_Line");
        hpImages[2].sprite = resource.GetSpriteAtlas("Bar_Atlas", "HP_Line");

        expImages[0].sprite = resource.GetSpriteAtlas("Bar_Atlas", "XP_Bar");
        expImages[1].sprite = resource.GetSpriteAtlas("Bar_Atlas", "XP_Line");
        expImages[2].sprite = resource.GetSpriteAtlas("Bar_Atlas", "XP_Line");
    }
    #endregion
}
