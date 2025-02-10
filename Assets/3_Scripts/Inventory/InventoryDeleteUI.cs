using UnityEngine;
using UnityEngine.UI;

public class InventoryDeleteUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Decrease , 1:Increase, 2:Handle, 3:DeleteBtn, 4:Frame")] Image[] deleteImages;
    [SerializeField] GameObject deleteParent;
    [SerializeField] Slider deleteSlider;
    [SerializeField, Tooltip("0:Cnt, 1:Select")] Text[] deleteCntTexts;
    
    string[] deleteStrings;
    ItemData data = null;

    public bool IsActive()
    {
        return deleteParent.activeSelf;
    }

    public void Init()
    {
        SetImages();
        SetTexts();
    }

    public void SetTexts()
    {
        deleteStrings = new string[2];
        deleteStrings[0] = "회수 수량 : ";
        deleteStrings[1] = "선택됨 : ";
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        deleteImages[0].sprite = res.GetSpriteAtlas("Icon_Atlas", "Red_Minus_Icon");
        deleteImages[1].sprite = res.GetSpriteAtlas("Icon_Atlas", "Red_Plus_Icon");
        deleteImages[2].sprite = res.GetSpriteAtlas("Icon_Atlas", "Red_Empty");
        deleteImages[3].sprite = res.GetSpriteAtlas("Button_Atlas", "Black_Frame");
        deleteImages[4].sprite = res.GetSpriteAtlas("Bar_Atlas_2", "Delete_Bar");
    }

    public void Active()
    {
        data = SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.CurrentItemData;

        if (data == null)
        {
            deleteSlider.value = 1;
            deleteSlider.maxValue = 1;
            deleteCntTexts[0].text = deleteStrings[0] + "0";
            deleteCntTexts[1].text = deleteStrings[1] + "0";
            if (deleteParent.activeSelf == false)
                deleteParent.SetActive(true);
        }
        else
        {
            deleteSlider.value = 1;
            deleteSlider.maxValue = data.itemCnt;
            deleteCntTexts[0].text = deleteStrings[0] + (int)deleteSlider.value;
            deleteCntTexts[1].text = deleteStrings[1] + (int)deleteSlider.value + "/" + data.itemCnt;
        }
        if (deleteParent.activeSelf == false)
            deleteParent.SetActive(true);
    }

    public void OnValueChange(float _value)
    {
        if (data == null)
            return;
        deleteCntTexts[0].text = deleteStrings[0] + (int)deleteSlider.value;
        deleteCntTexts[1].text = deleteStrings[1] + (int)deleteSlider.value + "/" + (int)deleteSlider.maxValue;
    }

    #region Btn

    public void PressPlusBtn()
    {
        if (data != null)
            deleteSlider.value +=1 ;
    }

    public void PressMinusBtn()
    {
        if(data!=null)
            deleteSlider.value -=1;
    }

    public void PressDeleteBtn()
    {
        if (data == null)
            return;

        data.Remove((int)deleteSlider.value);
        InActive();
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
    }
    #endregion

    public void InActive() 
    {
        data = null;
        deleteParent.SetActive(false);
    }
}

#region 
//public void Active(ConsumeData _data)
//{
//    data = _data;
//    deleteSlider.value = 1;
//    deleteSlider.maxValue = _data.itemCnt;
//    deleteCntTexts[0].text = deleteStrings[0] + (int)deleteSlider.value;
//    deleteCntTexts[1].text = deleteStrings[1] + (int)deleteSlider.value + "/" + _data.itemCnt;
//    if (deleteParent.activeSelf == false)
//        deleteParent.SetActive(true);
//}

//public void Active(WeaponData _data)
//{
//    data = _data;
//    deleteSlider.value = 1;
//    deleteSlider.maxValue = _data.itemCnt;
//    deleteCntTexts[0].text = deleteStrings[0] + (int)deleteSlider.value;
//    deleteCntTexts[1].text = deleteStrings[1] + (int)deleteSlider.value + "/" + _data.itemCnt;
//    if (deleteParent.activeSelf == false)
//        deleteParent.SetActive(true);
//}
#endregion