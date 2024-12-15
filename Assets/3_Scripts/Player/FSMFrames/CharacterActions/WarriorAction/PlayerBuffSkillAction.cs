using System.Collections;
using UnityEngine;

// 인게임에선 16진수를 사용

public class PlayerBuffSkillAction : BuffAction
{
    PlayerStat playerStat = null;
    PlayerBuffActionSOData soData = null;
    BuffData[] buffDatas;
    public void SetStat(PlayerStat _playerStat, PlayerBuffActionSOData _soData, CharacterStatCtrl _statCtrl)
    {
        playerStat = _playerStat;
        soData = _soData;
        statCtrl = _statCtrl;
        buffDatas = new BuffData[_soData.GetBuffCnt()];
    }

    public override void DoBuff()
    {
        int buffCnt = soData.GetBuffCnt();
        for(int i = 0; i < buffCnt; i++) 
        {
            statCtrl.AddBuffs(buffDatas[i]);
        }
    }

    public override void StopBuff()
    {
        int buffCnt = soData.GetBuffCnt();
        for (int i = 0; i < buffCnt; i++)
        {
            // Delete Buff
            //statCtrl.AddBuffs(buffDatas[i]);
        }
    }
}
