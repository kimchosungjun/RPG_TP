using ItemTableClassGroup;
using PlayerTableClassGroup;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManageView : MonoBehaviour
{
    [SerializeField] WeaponManageSlot[] slots;
    [SerializeField] GameObject[] infoParents;
    [SerializeField, Tooltip("0:Name, 1:atkInfo, 2: addInfo,3:Lv, 4:Hold")] Text[] curInfoTexts;
    [SerializeField, Tooltip("0:curLv, 1: maxLv,2:nextAtkIncrease, 3:addIncrease")] Text[] nextInfoTexts;
    [SerializeField] ReplaceWeaponButton replaceButton;
    [SerializeField] SelectWeaponLevelupButton levelUpButton;

    public void Init(Sprite _frameSprite)
    {
        int cnt = slots.Length;
        for (int i = 0; i < cnt; i++)
        {
            slots[i].Init(_frameSprite);
        }

        ClearWeaponInfos();
    }

    public void SetDataToWeaponList(List<WeaponData> _weapons)
    {
        List<WeaponData> dataSet = _weapons;
        int dataSetCnt = 0;
        int slotCnt = slots.Length;
        if (_weapons != null)
            dataSetCnt = dataSet.Count;
        for (int i = 0; i < dataSetCnt; i++)
        {
            slots[i].SetSlot(dataSet[i]);
        }
        for (int i = dataSetCnt; i < slotCnt; i++)
        {
            slots[i].SetSlot();
        }
    }

    public void PressWeaponSlot(WeaponData _data)
    {
        if (_data == null)
            return;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerPartyUI.GetWeaponManageUI.CurrentSelectWeaponData = _data;
        int curLevel  = _data.weaponCurrentLevel;
        int maxLevel = _data.weaponMaxLevel;
        curInfoTexts[0].text = _data.itemName;
        curInfoTexts[1].text = "공격력 : " +_data.attackValue.ToString();
        float effectValue = _data.effectValue *100;
        curInfoTexts[2].text =_data.GetAdditionalEffectName()+ " : " + effectValue.ToString("F1") +"%";
        curInfoTexts[3].text = "Lv. "+ curLevel;
        
        if (_data.IsHoldWeapon == false)
            curInfoTexts[4].text = string.Empty;
        else
        {
            PlayerTableData playerData = SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(_data.holdPlayerID);
            if(playerData == null)
                curInfoTexts[4].text = string.Empty;
            else
                curInfoTexts[4].text = $"{playerData.name} 착용중";
        }

        nextInfoTexts[0].text = "Lv. " + curLevel;
        nextInfoTexts[1].text = "Lv. " + maxLevel;

        if (curLevel == maxLevel)
        {
            levelUpButton.DecideInteract(false);
            nextInfoTexts[2].text = string.Empty;
            nextInfoTexts[3].text = string.Empty;
        }
        else
        {
            WeaponTableData weaponTableData = SharedMgr.TableMgr.GetItem.GetWeaponTableData(_data.itemID);
            float addValue = weaponTableData.increaseAdditionEffectValue * 100f;
            levelUpButton.DecideInteract(true);
            nextInfoTexts[2].text = "레벨 별 공격력 증가량 : "+ weaponTableData.increaseAttackValue;
            nextInfoTexts[3].text = "레벨 별 추가 효과 증가량 : "+ addValue.ToString("F1") + "%";
        }

        if (infoParents[0].activeSelf ==false)
            infoParents[0].SetActive(true);
        if (infoParents[1].activeSelf == false)
            infoParents[1].SetActive(true);
    }

    public void ClearWeaponInfos()
    {
        if (infoParents[0].activeSelf)
            infoParents[0].SetActive(false);
        if (infoParents[1].activeSelf)
            infoParents[1].SetActive(false);
    }
}
