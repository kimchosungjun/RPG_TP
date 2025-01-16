using UnityEngine;
using UnityEngine.UI;

public class SelectWeaponLevelupButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Button button;
    public void Init()
    {
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        if(button ==null) button = GetComponent<Button>();
        buttonImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas_2", "Red_Long_Bar");
    }

    public void PressButton()
    {
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetWeaponManageUI.SelectLevelUpButton();
    }

    public void DecideInteract(bool _canInteract)
    {
        button.interactable = _canInteract;
    }
}
