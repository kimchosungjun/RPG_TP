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
        statusCanvasObject.SetActive(true);
    }

    public void InActive()
    {
        isActive = false;
        statusCanvasObject.SetActive(false);
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

    /****************************/
    /******* Set Data**********/
    /****************************/

    #region Set Data & Link UI 
    public override void Init()
    {
        if (statusCanvasObject.activeSelf) statusCanvasObject.SetActive(false);
        SetImages();
    }

    public void Setup(PlayerStat _playerStat)
    {
        if (isActive == false)
            return;
        //set hp
        stat = _playerStat;
        float currentHP = _playerStat.GetSaveStat.currentHP;
        float maxHp = _playerStat.MaxHP;
        hpImages[1].fillAmount = currentHP/maxHp;
        hpImages[2].fillAmount = hpImages[1].fillAmount;
        hpText.text = _playerStat.GetSaveStat.currentHP + "/" + _playerStat.MaxHP;
        levelText.text = "Lv."+_playerStat.GetSaveStat.currentLevel;
    }

    public void UpdateData(PlayerStat _playerStat)
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
        if (statusCanvasObject.activeSelf == false) statusCanvasObject.SetActive(true);
    }
    #endregion

    /****************************/
    /****** Update Data ******/
    /****************************/

    #region Update UI

    bool updateHp = false;
    public void UpdateHP()
    {
        hpTarget = stat.GetSaveStat.currentHP;
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
    #endregion
}
