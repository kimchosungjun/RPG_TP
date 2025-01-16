using UnityEngine;
using UnityEngine.UI;

public class ReplaceWeaponButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Button button;
    public void Init()
    {
        if(buttonImage==null) buttonImage = GetComponent<Image>();
        if(button ==null) button = GetComponent<Button>();  
        buttonImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "StartGame_Frame");
    }

    public void PressButton()
    {
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetWeaponManageUI.ChangeHoldWeapon();
    }

    public void DecideInteract(bool _canInteract)
    {
        button.interactable = _canInteract;
    }
}
