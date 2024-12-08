using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "PlayerBuffAction", menuName = "Player/Action/Buff")]
public class PlayerBuffActionData : PlayerActionData
{
    [Header("버프"), SerializeField] protected PlayerBuffData[] buffDatas;
    public PlayerBuffData[] GetBuffData() { return buffDatas; }
}