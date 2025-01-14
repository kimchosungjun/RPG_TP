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
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetPlayerPartyStatusUI.PressCharacter(characterID);
    }
}
