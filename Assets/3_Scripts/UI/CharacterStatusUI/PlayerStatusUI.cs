using UnityEngine;
using UnityEngine.UI;
using UIEnums;
using System.Collections;

public class PlayerStatusUI : StatusUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [SerializeField] Text hpText;
    [SerializeField, Header("HP Bar"), Tooltip("0:Frame, 1:Fill, 2:Effect")] Image[] hpImages;
    [SerializeField, Header("Exp Bar"), Tooltip("0:Frame, 1:Fill, 2:Effect")] Image[] expImages;
    [SerializeField] Text expText;
    [SerializeField] PlayerJoystickUI joystickUI;

    public PlayerJoystickUI GetJoystickUI { get { return joystickUI; } }

    int curLv = -1;
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
        TurnOff();
        SetImages();
        joystickUI.Init();
    }

    public void ChangeData(PlayerStat _playerStat)
    {
        // update bool init
        curLv = _playerStat.GetSaveStat.currentLevel;
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
        {
            expText.text = string.Empty;
            expImages[1].fillAmount = 1f;
        }
        else
        {
            expText.text = stat.GetSaveStat.currentExp + "/" + maxExp;
            expImages[1].fillAmount = _playerStat.GetSaveStat.currentExp / maxExp;
            expImages[2].fillAmount = expImages[1].fillAmount;
        }
        // set text
        hpText.text = (int)currentHP + "/" + (int)maxHp;
        levelText.text = "Lv." + _playerStat.GetSaveStat.currentLevel;
        

        TurnOn();
    }
    #endregion

    /******************************************/
    /************ Update Data **************/
    /******************************************/

    #region Update UI

    public void UpdateData(STATUS _updateStatusType)
    {
        switch (_updateStatusType)
        {
            case STATUS.HP:
                UpdateStatusData();
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
    public override void UpdateStatusData()
    {
        hpTarget = stat.GetSaveStat.currentHP;
        hpText.text = hpTarget + "/" + stat.MaxHP;
        hpImages[1].fillAmount = (float)hpTarget/stat.MaxHP;
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
        if (stat.GetSaveStat.currentLevel < SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().maxLevel)
            maxExp = SharedMgr.TableMgr.GetPlayer.GetPlayerLevelTableData().needExps[stat.GetSaveStat.currentLevel - 1];
        else
            maxExp = -1;
        if (maxExp < 0)
        {
            expText.text = string.Empty;
            expImages[1].fillAmount = 1;
            expImages[2].fillAmount = 1;
            return;
        }
        expImages[2].fillAmount = (float)stat.GetSaveStat.currentExp / maxExp;
        expText.text = stat.GetSaveStat.currentExp + "/" + maxExp;
        if(curLv != stat.GetSaveStat.currentLevel)
        {
            UpdateData(STATUS.HP);
            UpdateData(STATUS.LEVEL);
        }
        if (updateExp == false)
        {
            updateExp = true;
            StartCoroutine(CUpdateExp());
        }
    }
    IEnumerator CUpdateExp()
    {
        while (true)
        {
            if (expImages[2].fillAmount <= expImages[1].fillAmount)
            {
                updateExp = false;
                expImages[1].fillAmount = expImages[2].fillAmount;
                break;
            }
            expImages[1].fillAmount += Time.fixedDeltaTime / effectTime;
            yield return new WaitForFixedUpdate();
        }
    }
    public void UpdateLevel()
    {
        levelText.text = "Lv."+stat.GetSaveStat.currentLevel;
        curLv = stat.GetSaveStat.currentLevel;
    }
    #endregion

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface Method
    public override void TurnOn()
    {
        statusParentObject.SetActive(true);
    }

    public override void TurnOff()
    {
        statusParentObject.SetActive(false);
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
