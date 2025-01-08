using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChangeButton : MonoBehaviour
{
    [SerializeField, Header("Button Image"), Tooltip("0:Circle, 1:Frame, 2:Icon, 3:Effect, 4 : Panel")] Image[] changeButtonImages;
    [SerializeField, Header("Lv Text")] Text levelText;
    [SerializeField] Button button;
    public void SetImage()
    {
        changeButtonImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Circle_Frame");
        changeButtonImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Player_Frame");
    }

    public void ChangeButtonData(int _level, string _name)
    {
        button.interactable = true;
        levelText.text = _level.ToString();
        string fileName = _name + "_Icon";
        changeButtonImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", fileName);
        if (levelText.gameObject.activeSelf == false)
            levelText.gameObject.SetActive(true);
        if (changeButtonImages[2].gameObject.activeSelf == false)
            changeButtonImages[2].gameObject.SetActive(true);
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
}
