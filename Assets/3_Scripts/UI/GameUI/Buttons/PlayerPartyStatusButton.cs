using UnityEngine;
using UnityEngine.UI;

public class PlayerPartyStatusButton : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:Icon")] Image[] buttonImages;
    [SerializeField] Button button;
    int characterID = -1;
    Color halfColor = Color.white;

    public void Init()
    {
        SetImage();
    }

    public void SetImage()
    {
        halfColor.a = 0.5f;
        buttonImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Btn_Empty_Frame");
        buttonImages[1].gameObject.SetActive(false);
        buttonImages[1].color = halfColor;
        button.interactable = false;
    }

    public void SetButton(int _id, Sprite _icon)
    {
        if (characterID == _id) return;
        characterID = _id;
        buttonImages[1].color = Color.white;
        buttonImages[1].sprite = _icon;
        if(buttonImages[1].gameObject.activeSelf==false)
            buttonImages[1].gameObject.SetActive(true);
        button.interactable = true;
    }

    public void PressButton()
    {
        if (characterID == -1) return;
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetPlayerPartyStatusUI.PressCharacter(characterID);
        SetEffectRect();
    }

    public void PressUnderCharacterButton()
    {
        if (characterID == -1) return;
        SharedMgr.SoundMgr.PressButtonSFX();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.InputUnderCharacterButton(characterID);
        SetEffectRect();
    }

    public void SetEffectRect()
    {
        RectTransform rect = SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetEffectTransform;
        if (rect == null) return;

        rect.SetParent(this.transform);
        rect.anchoredPosition = Vector2.zero;
        if (rect.gameObject.activeSelf == false)
            rect.gameObject.SetActive(true);
    }
}
