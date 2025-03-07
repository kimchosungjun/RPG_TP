using ItemEnums;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace ItemStrategy
{
    public abstract class ItemInteract
    {
        public abstract void Use(int _value = 1);
        public abstract void Remove(int _cnt = 1);
    }

    public class EtcInteract : ItemInteract
    {
        EtcData data = null;

        public EtcInteract(EtcData _data) { data = _data; }

        public override void Use(int _value = 1) 
        {
            data.itemCnt -= _value;
            if (data.itemCnt <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
        }

        public override void Remove(int _cnt = 1)
        {
            data.itemCnt -= _cnt;
            SharedMgr.InventoryMgr.AddGold(_cnt * data.etcExp);
            if (data.itemCnt <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
        }
    }

    public class ConsumeInteract : ItemInteract
    {
        ConsumeData data = null;

        public ConsumeInteract(ConsumeData _data) { data = _data; }

        public override void Use(int _value = 1)
        {
            data.itemCnt -= _value;
            BasePlayer player = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer;
            TransferConditionData conditionData = new TransferConditionData();
            conditionData.SetData(player.PlayerStat, data.effectStat, data.attributeStat, data.duration, data.defaultValue, data.maintainTime, data.multiplier, data.applyStatType);
            player.GetPlayerStatControl.AddCondition(conditionData);
            if (data.itemCnt <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
            else
                SharedMgr.UIMgr.GameUICtrl.GetInventoyUI.UpdateInventory();
        }

        public override void Remove(int _cnt = 1)
        {
            data.itemCnt -= _cnt;
            if (data.itemCnt <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
        }
    }

    public class WeaponInteract : ItemInteract
    {
        WeaponData data;
        public WeaponInteract(WeaponData _data) { data = _data; }

        public override void Use(int _value = 1)
        {
            data.IsHoldWeapon = true;
            data.holdPlayerID = _value;
            WeaponIncreaseStat weaponStat = new WeaponIncreaseStat();
            PlayerStat playerStat = SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat(_value);
            if (playerStat == null)
                return;
            switch (data.WeaponEffect)
            {
                case WEAPONEFFECT.WEAPON_ATTACK:
                    int increaseValue = (int)(playerStat.Attack * data.effectValue);
                    weaponStat.SetValues(increaseValue + data.attackValue, 0);
                    break;
                case WEAPONEFFECT.WEAPON_CRITICAL:
                    weaponStat.SetValues(data.attackValue, data.effectValue);
                    break;
            }
            SharedMgr.GameCtrlMgr.GetPlayerStatCtrl.GetPlayerStat(_value).ApplyWeaponStat(weaponStat);
        }

        public override void Remove(int _cnt = 1)
        {
            SharedMgr.InventoryMgr.AddGold(data.weaponPrice);
            SharedMgr.InventoryMgr.RemoveItem(data);
            UniqueIDMaker.RemoveID(data.uniqueID);
        }
    }
}
