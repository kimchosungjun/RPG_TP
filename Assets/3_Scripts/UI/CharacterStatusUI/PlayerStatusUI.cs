using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : StatusUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [SerializeField] Text hpText;
    [SerializeField] Image expImage;
    float expTarget = 0f;
    float maxExp = 0f;
    #endregion


    /******************************************/
    /******* 라이프 사이클 재정의 *********/
    /******************************************/

    #region Override Life Cycle
    public override void Init()
    {
        if (statusCanvasObject.activeSelf) statusCanvasObject.SetActive(false);
    }

    public void Setup(PlayerStat _playerStat)
    {
        float currentHP = _playerStat.GetSaveStat.currentHP;
        float maxHp = _playerStat.HP;
        hpImage.fillAmount = currentHP/maxHp;
        effectImage.fillAmount = hpImage.fillAmount;
        hpText.text = (int)currentHP + "/" + (int)maxHp;
        levelText.text = "Lv."+_playerStat.GetSaveStat.currentLevel;

        maxExp = SharedMgr.TableMgr.Player.GetPlayerLevelTableData().needExps[_playerStat.GetSaveStat.currentLevel - 1];
        if(maxExp < 0f)
        {
            // 최대레벨에 도달했음
            expImage.fillAmount = 1f;
        }
        else
        {
            expTarget = _playerStat.GetSaveStat.currentExp;
            expImage.fillAmount = expTarget / maxExp;
        }
         
        if (statusCanvasObject.activeSelf == false) statusCanvasObject.SetActive(true);
    }

    public void UpdateData(PlayerStat _playerStat)
    {
        float currentHP = _playerStat.GetSaveStat.currentHP;
        float maxHp = _playerStat.HP;
        hpImage.fillAmount = currentHP / maxHp;
        hpText.text = (int)currentHP + "/" + (int)maxHp;
        levelText.text = "Lv." + _playerStat.GetSaveStat.currentLevel;
    }



    public override void FixedExecute()
    {
        HPEffect();
        EXPEffect();
    }

    public virtual void EXPEffect()
    {
        if (expImage.fillAmount == expTarget) return;

        if (expImage.fillAmount < expTarget) expImage.fillAmount += Time.deltaTime / effectTime;
        else if (expImage.fillAmount > expTarget) effectImage.fillAmount = expTarget;
    }
    #endregion
}
