using UnityEngine;
using UnityEngine.UI;


public class WeaponManageSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame,1:Icon")] Image[] slotImages;
    [SerializeField] Text levelText;
    [SerializeField] Button button;
    WeaponData data = null;
    public void Init(Sprite _sprite)
    {
         levelText.text = string.Empty;
        button.interactable = false;
         SetImages(_sprite);
    }
    
    public void SetImages(Sprite _sprite)
    {
        slotImages[0].sprite = _sprite;
        if (slotImages[1].gameObject.activeSelf)
            slotImages[1].gameObject.SetActive(false);
    }
    
    public void SetSlot(WeaponData _data)
    {
        if (data == _data) return;
        data = _data;   
        slotImages[1].color = Color.white;
        slotImages[1].sprite = _data.GetIcon; 
        if (slotImages[1].gameObject.activeSelf==false)
            slotImages[1].gameObject.SetActive(true);
        button.interactable = true;
    }

    public void SetSlot()
    {
        if (data == null) return;
        data = null;
        if (slotImages[1].gameObject.activeSelf)
            slotImages[1].gameObject.SetActive(false);
        levelText.text = string.Empty;
        button.interactable = false;
    }

    public void PressButton()
    {
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetWeaponManageUI.GetManageView.PressWeaponSlot(data);
     }
}
