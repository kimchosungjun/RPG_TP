using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGetItemSlot : MonoBehaviour
{
    [SerializeField] Image slotFrameImage;
    [SerializeField] Image iconImage;
    [SerializeField] Text getText;
    [SerializeField] Animator anim;
    WaitForSeconds showTime = new WaitForSeconds(2f);
    Sprite moneyIcon = null;

    public void ShowSlot(ItemData _itemData)
    {
        if (_itemData.IsMoney())
        {
            ShowSlot(_itemData.itemCnt);
            return;
        }
        iconImage.sprite = _itemData.GetIcon;
        getText.text = _itemData.itemName + " X " + _itemData.itemCnt;
        this.gameObject.SetActive(true);
    }

    public void ShowSlot(int _GetMoney)
    {
        iconImage.sprite = moneyIcon;
        getText.text = $"골드 X {_GetMoney}";
        this.gameObject.SetActive(true);
    }

    public void Active()
    {
        StartCoroutine(CActive());
    }

    IEnumerator CActive()
    {
        iconImage.gameObject.SetActive(true);
        getText.gameObject.SetActive(true);
        yield return showTime;
        StartCoroutine(CInActive());    
    }

    public void InActive()
    {
        this.gameObject.SetActive(false);
        SharedMgr.InventoryMgr.ShowNextGetItemSlot();
    }

    IEnumerator CInActive()
    {
        anim.Play("GetSlotInActive");
        yield break;
    }

    internal void SetImages()
    {
        moneyIcon = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_2", "Gold_Icon");
        slotFrameImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Slot_Atlas","Get_Item");
    }
}
