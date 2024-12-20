// 인게임에선 16진수를 사용
using UnityEngine;

public class PlayerBuffSkillAction : BuffAction
{
    PlayerStat playerStat = null;
    PlayerBuffActionSOData soData = null;
    TransferConditionData[] buffDatas;
    public virtual void SetStat(PlayerStat _playerStat, PlayerBuffActionSOData _soData, ActorStatControl _statCtrl)
    {
        playerStat = _playerStat;
        soData = _soData;
        statCtrl = _statCtrl;

        int buffCnt = _soData.GetBuffCnt(); 
        buffDatas = new TransferConditionData[buffCnt];
        for(int i = 0; i < buffCnt; i++)
        {
            buffDatas[i] = new TransferConditionData();
            //buffDatas[i].SetData(_soData.GetEffectStatType(i), _soData.GetUseStatType(i),
            //    _soData.GetContinuityType(i), _soData.GetMultiplier(i), _soData.GetMaintainEffectTime);
        }
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
