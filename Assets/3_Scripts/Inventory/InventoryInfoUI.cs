using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemEnums;
using TMPro;

public class InventoryInfoUI : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] GameObject[] InfoObject;
    int currentIndex = 0;

    public void TurnOffCurrentInfo()
    {
        InfoObject[currentIndex].SetActive (false);    
    }

    #region Etc
    [Header("Etc")]
    [SerializeField] Image etcIcon;
    [SerializeField] Image etcTypeIcon;
    [SerializeField] Text etcName;
    [SerializeField] Text etcCntText;
    [SerializeField] Text etcDescriptionText;

    public void ShowInfo(EtcData _itemData)
    {
        currentIndex = (int)ITEMTYPE.ITEM_ETC;
        etcIcon.sprite = _itemData.itemIcon;
        etcTypeIcon.sprite = _itemData.itemTypeIcon;
        etcName.text = _itemData.itemName;
        etcCntText.text = "X"+_itemData.itemCnt;
        etcDescriptionText.text = _itemData.itemDescription;
        InfoObject[currentIndex].SetActive(true);
    }
    #endregion

    #region Consume

    [Header("Consume")]
    [SerializeField] Image consumeIcon;
    [SerializeField] Image consumeTypeIcon;
    [SerializeField] Text consumeName;
    [SerializeField] Text consumeCntText;
    [SerializeField] Text consumeDescriptionText;
    ConsumeData consumeData = null;

    public void ShowInfo(ConsumeData _itemData)
    {
        consumeData = _itemData;
        currentIndex = (int)ITEMTYPE.ITEM_COMSUME;
        InfoObject[currentIndex].SetActive(true);
    }

    public void PressUse()
    {
        SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.DecideUse(consumeData);
    }

    #endregion

    #region Weapon

    [Header("Weapon")]
    [SerializeField] Image weaponIcon;
    [SerializeField] Image weaponTypeIcon;
    [SerializeField] Text weaponName;
    [SerializeField] Text weaponDescriptionText;
    [SerializeField] Slider weaponLevelSlide;
    [SerializeField] Text weaponLevelText;
    [SerializeField] Text weaponAttackText;
    [SerializeField] Text[] weaponAdditionTexts;

    public void ShowInfo(WeaponData _itemData)
    {
        currentIndex = (int)ITEMTYPE.ITEM_WEAPON;
        weaponIcon.sprite = _itemData.itemIcon;
        weaponTypeIcon.sprite = _itemData.itemTypeIcon;
        weaponName.text = _itemData.itemName;
        weaponDescriptionText.text = _itemData.itemDescription;
        weaponLevelText.text = "Lv."+_itemData.weaponCurrentLevel;
        if (_itemData.weaponMaxExp == 0)
            weaponLevelSlide.value = 1;
        else
            weaponLevelSlide.value = _itemData.weaponCurrentExp / _itemData.weaponMaxExp;
        weaponAttackText.text = _itemData.attackValue.ToString();
     
        switch (_itemData.WeaponEffect)
        {
            case WEAPONEFFECT.WEAPON_ATTACK:
                weaponAdditionTexts[0].text = "공격력 % 증가";
                break;
            case WEAPONEFFECT.WEAPON_CRITICAL:
                weaponAdditionTexts[0].text = "크리티컬 증가";
                break;
        }
        weaponAdditionTexts[1].text = (int)(_itemData.effectValue * 100) + "%";
        InfoObject[currentIndex].SetActive(true);
    }
    #endregion

}
