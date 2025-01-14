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
    public void Init(Sprite _sprite)
    {
        halfColor.a = 0.5f;
        levelText.text = string.Empty;
        button.interactable = false;
        slotImages[1].color = halfColor;
        SetImages(_sprite);
    }
    
    public void SetImages(Sprite _sprite)
    {
        slotImages[0].sprite = _sprite;
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
