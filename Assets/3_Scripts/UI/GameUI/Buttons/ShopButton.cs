using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] Image image;
    ItemData itemData = null;

    public void SetData(ItemData _itemData)
    {
        if(_itemData==null)
        {
            itemData = null;
            image.gameObject.SetActive(false);
            return;
        }
        itemData = _itemData;
        image.sprite = _itemData.GetIcon;
    }

    public void PressButton()
    {
        if (itemData == null)
            return;
    }
}
