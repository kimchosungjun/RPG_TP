using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChangeButton : MonoBehaviour
{
    [SerializeField, Header("Button Image"), Tooltip("0:Circle, 1:Frame, 2:Icon, 3:Effect, 4 : Panel, 5: DeathPanel")] Image[] changeButtonImages;
    [SerializeField, Header("Lv Text")] Text levelText;
    [SerializeField] Button button;
    [SerializeField] int index;
  
    PlayerStat stat = null;
    public void SetImage()
    {
        changeButtonImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Circle_Frame");
        changeButtonImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Player_Frame");
        changeButtonImages[5].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Circle_Frame");
    }

    public void ChangeButtonData(PlayerStat _playerStat)
    {
        stat = _playerStat; 
        button.interactable = true;
        levelText.text = stat.GetSaveStat.currentLevel.ToString();
        string fileName = SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(stat.GetSaveStat.playerTypeID).prefabName + "_Icon";
        changeButtonImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", fileName);
        if (levelText.gameObject.activeSelf == false)
            levelText.gameObject.SetActive(true);
        if (changeButtonImages[2].gameObject.activeSelf == false)
            changeButtonImages[2].gameObject.SetActive(true);
    }

    public void UpdateLiveState()
    {
        if (stat == null) return;
        if (stat.GetSaveStat.currentHP <= 0)
            changeButtonImages[5].gameObject.SetActive(true);
        else
            changeButtonImages[5].gameObject.SetActive(false);
    }

    public void AliveState() { changeButtonImages[5].gameObject.SetActive(false); }
   

    public void UpdateLevel()
    {
        if (stat == null) return;
        levelText.text = stat.GetSaveStat.currentLevel.ToString();
    }

    public void EmptyButton()
    {
        button.interactable = false;
        if (levelText.gameObject.activeSelf)
            levelText.gameObject.SetActive(false);
        if (changeButtonImages[2].gameObject.activeSelf)
            changeButtonImages[2].gameObject.SetActive(false);
    }

    public Image GetPanel() { return changeButtonImages[4]; }
    public void ControlEffect(bool _isActive) { changeButtonImages[3].gameObject.SetActive(_isActive); }
    public void PressPlayerChangeButton() { SharedMgr.GameCtrlMgr.GetPlayerCtrl.PressChangePlayer(index); }
}
