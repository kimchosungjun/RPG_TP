using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGetItemSlot : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] Text getText;
    [SerializeField] Animator anim;
    WaitForSeconds showTime = new WaitForSeconds(2f);

    public void ShowSlot(ItemData _itemData)
    {
        iconImage.sprite = _itemData.itemIcon;
        getText.text = _itemData.itemName + " X " + _itemData.itemCnt;
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
        iconImage.gameObject.SetActive(false);
        getText.gameObject.SetActive(false);
        anim.Play("GetSlotInActive");
        yield break;
    }
}
