using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeaponManageSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame,1:Icon")] Image[] slotImages;
    [SerializeField] Text levelText;
    [SerializeField] Button button;
    Color halfColor = Color.white;  
    public void Init()
    {
        halfColor.a = 0.5f;
        levelText.text = string.Empty;
        button.interactable = false;
        slotImages[1].color = halfColor;
        SetImages();
    }
    
    public void SetImages()
    {
        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "WeaponManage_Slot_Button");
        if (slotImages[1].gameObject.activeSelf)
            slotImages[1].gameObject.SetActive(false);
    }

    public void SetSlot()
    {
        slotImages[1].color = Color.white;
        slotImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("", "");
        button.interactable = true;
    }
}
