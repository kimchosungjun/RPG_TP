using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer : BaseCharacter
{
    [SerializeField] protected PlayerStat playerStat;
    public PlayerStat GetPlayerStat { get { return playerStat; }  set { playerStat = value; } }

    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    #region Abstract Method
    public abstract void Init();
    public abstract void Setup();
    public abstract void Execute();
    public abstract void FixedExecute();
    #endregion
}
