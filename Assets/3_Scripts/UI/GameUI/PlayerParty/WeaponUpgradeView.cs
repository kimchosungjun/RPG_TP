using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeView : MonoBehaviour
{
    [SerializeField] WeaponUpgradeMatSlot matSlot;
    [SerializeField] Transform parentTransform;
    List<WeaponUpgradeMatSlot> slots = new List<WeaponUpgradeMatSlot>();

    [SerializeField, Tooltip("0:BackFrame, 1:EnhanceBtn, 2:ClearBtn, 3:AtkFrame, 4:AddFrame, 5:UseSlotFrame, 6:ExpSliderFrame")] Image[] images;
    [SerializeField,Tooltip(" 0:AtkIcon, 1:AddIcon, 2:Direction, 3:AddDirection,4:UseSlotIcon") ] Image[] icons;
    [SerializeField, Tooltip("0:WeaponName, 1:curLv, 2:NextLv, 3:CurAtk, 4:NextAtk, 5:curAdd, 6:nextAdd, 7: useMatCnt, 8: AddNameText")] Text[] texts;
    [SerializeField] Slider expSlider; 
    Sprite defaultUseItemIcon = null;
    WeaponData data = null;
    
    int matCnt = 0;
    EtcData matData = null;
    bool isMaxExp = false;
    public bool GetIsMaxExp { get { return isMaxExp; } }

    public void Init()
    {
        InitMatSlots();
        SetImages();
    }

    public void InitMatSlots()
    {
        List<EtcData> datas =  SharedMgr.InventoryMgr.GetEtcInventory();
        int dataCnt = datas.Count;
        for (int i = 0; i < dataCnt; i++)
        {
            GameObject go = Instantiate(matSlot.gameObject, parentTransform);
            slots.Add(go.GetComponent<WeaponUpgradeMatSlot>());
        }

        for (int i = 0; i < dataCnt; i++)
        {
            slots[i].Init(datas[i], this);
        }
    }


    public void UpdateMatSlots()
    {
        List<EtcData> datas = SharedMgr.InventoryMgr.GetEtcInventory();
        int dataCnt = datas.Count;
        int slotCnt = slots.Count;

        if (slotCnt < dataCnt)
        {
            int diff = dataCnt - slotCnt;
            for (int i = 0; i < diff; i++)
            {
                GameObject go = Instantiate(matSlot.gameObject, parentTransform);
                slots.Add(go.GetComponent<WeaponUpgradeMatSlot>());
            }

            for (int i = 0; i < dataCnt; i++)
            {
                slots[i].Init(datas[i], this);
            }
            return;
        }

        for (int i = 0; i < dataCnt; i++)
        {
            slots[i].SetData(datas[i]);
        }
        for (int i = dataCnt; i < slotCnt; i++)
        {
            slots[i].SetData();
        }
    }

    public void SetWeaponData(WeaponData _data)
    {
        data = _data;
        if(_data.weaponMaxExp == -1)
        {
            isMaxExp = true;
            expSlider.maxValue = 1f;
            expSlider.value = 1f;
        }
        else
        {
            isMaxExp = false;
            expSlider.maxValue = _data.weaponMaxExp;
            expSlider.value = _data.weaponCurrentExp;
        }

        texts[0].text = _data.itemName;
        texts[1].text = "Lv. "+_data.weaponCurrentLevel;
        texts[2].text = texts[1].text;
        texts[3].text = _data.attackValue.ToString();
        texts[4].text = texts[3].text;
        texts[5].text = _data.GetEffectValue().ToString("F1")+"%";
        texts[6].text = texts[5].text;
        texts[8].text = _data.GetAdditionalEffectName();
    }

    public void PredictAddExp(int _exp)
    {
        if (data == null) return;
        Tuple<int,int,int,float,int> predictData = data.PredictEnhance(_exp);
        texts[2].text = "Lv. " + predictData.Item1;
        if (predictData.Item5 == -1)
        {
            // max level
            isMaxExp = true;
            expSlider.maxValue = 1f;
            expSlider.value = 1f;
        }
        else
        {
            isMaxExp = false;
            expSlider.maxValue = predictData.Item5;
            expSlider.value = predictData.Item2;
        }

        texts[4].text = predictData.Item3.ToString();
        texts[6].text = predictData.Item4.ToString("F1")+"%";
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        images[0].sprite = res.GetSpriteAtlas("Window_Atlas", "WeaponUpgrade_Frame");
        images[1].sprite = res.GetSpriteAtlas("Bar_Atlas_2", "Red_Long_Bar");
        images[2].sprite = res.GetSpriteAtlas("Button_Atlas", "Black_Frame");
        Sprite indicateFrame = res.GetSpriteAtlas("Slot_Atlas", "Weapon_Manage_Frame");
        images[3].sprite = indicateFrame;
        images[4].sprite = indicateFrame;
        images[5].sprite = res.GetSpriteAtlas("Slot_Atlas", "Item_Icon_Frame");
        images[6].sprite = res.GetSpriteAtlas("Bar_Atlas", "Loading_Bar");

        Sprite weaponIcon = res.GetSpriteAtlas("Icon_Atlas", "Weapon_Icon");
        Sprite directionIcon = res.GetSpriteAtlas("Icon_Atlas", "Next_Icon");
        icons[0].sprite = weaponIcon;
        icons[1].sprite = weaponIcon;
        icons[2].sprite = directionIcon;
        icons[3].sprite = directionIcon;

        defaultUseItemIcon = res.GetSpriteAtlas("Icon_Atlas_3", "Type_Etc");
        icons[4].sprite = defaultUseItemIcon;
        texts[7].text = "0";
    }

    #region Button Function
    public void PressMatEctButton(EtcData _data)
    {
        if (_data == null)
        {
            Debug.LogError("Error Mat Data!!");
            return;
        }

        if (isMaxExp) 
        {
            Debug.Log("현재 최대 경험치");
            return;
        }

        if (matData == null)
        {
            matData = _data;
            matCnt += 1;
            icons[4].sprite = _data.GetIcon;
            PredictAddExp(matData.etcExp);
        }
        else
        {
            if(_data == matData)
            {
                matCnt = (matCnt + 1) > _data.itemCnt ? _data.itemCnt : matCnt + 1;
                PredictAddExp(matData.etcExp * matCnt);
            }
            else
            {
                matCnt = 1;
                matData = _data;
                icons[4].sprite = _data.GetIcon;
                PredictAddExp(matData.etcExp);
            }
        }
        texts[7].text = matCnt.ToString();
    }

    public void PressUseMatSlotButton()
    {
        if (matCnt == 0 || matData==null) return;
        matCnt -= 1;
        if(matCnt<=0)
            ClearUseMatSlot();
        else
        {
            texts[7].text = matCnt.ToString();
            PredictAddExp(matData.etcExp * matCnt);
        }
    }

    public void ClearUseMatSlot()
    {
        icons[4].sprite = defaultUseItemIcon;
        texts[7].text = "0";
        matCnt = 0;
        matData = null;
        PredictAddExp(0);
    }

    public void DecideEnhanceWeapon()
    {
        if (matData == null) return;
        data.ApplyEnhance(matCnt * matData.etcExp);
        matData.Use(matCnt);
        ClearUseMatSlot();
        SetWeaponData(data);
        UpdateMatSlots();
    }
    #endregion
}
