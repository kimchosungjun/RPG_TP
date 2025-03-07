using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace ItemStrategy
{
    public abstract class ItemInteract
    {
        public abstract void Use(int _cnt = 1);
        public abstract void Remove(int _cnt = 1);
    }

    public class EtcInteract : ItemInteract
    {
        EtcData data = null;

        public EtcInteract(EtcData _data) { data = _data; }

        public override void Use(int _cnt = 1) 
        {
            data.itemID -= _cnt;
            if (data.itemID <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
        }

        public override void Remove(int _cnt = 1)
        {
            data.itemID -= _cnt;
            SharedMgr.InventoryMgr.AddGold(_cnt * data.etcExp);
            if (data.itemID <= 0)
                SharedMgr.InventoryMgr.RemoveItem(data);
        }
    }

    public class ConsumeInteract : ItemInteract
    {
        ConsumeData data = null;

        public override void Use(int _cnt = 1)
        {
            //_itemData.itemID -= _cnt;
            //if (_itemData.itemID <= 0)
            //    SharedMgr.InventoryMgr.RemoveItem(_itemData as EtcData);
        }

        public override void Remove(int _cnt = 1)
        {
            //_itemData.itemID -= _cnt;
            //if (_itemData.itemID <= 0)
            //    SharedMgr.InventoryMgr.RemoveItem(_itemData as EtcData);
        }
    }

    public class WeaponInteract : ItemInteract
    {
        public override void Use(int _cnt = 1)
        {
            //_itemData.itemID -= _cnt;
            //if (_itemData.itemID <= 0)
            //    SharedMgr.InventoryMgr.RemoveItem(_itemData as EtcData);
        }

        public override void Remove(int _cnt = 1)
        {
            //_itemData.itemID -= _cnt;
            //if (_itemData.itemID <= 0)
            //    SharedMgr.InventoryMgr.RemoveItem(_itemData as EtcData);
        }
    }
}
