using EffectEnums;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorStatControl : MonoBehaviour
{
    /******************************************/
    /*************** 버프 & UI ***************/
    /******************************************/

    #region Value : Buff & StatusUI
    protected int buffCnt = 0;
    Dictionary<BUFF_APPLY_STATS, int> overlapBuffGroup = new Dictionary<BUFF_APPLY_STATS, int>();
    protected List<TransferConditionData> currentBuffs = new List<TransferConditionData>();
    protected StatusUI statusUI = null;

    public void SetStatusUI(StatusUI _statusUI) { this.statusUI = _statusUI; }
    #endregion

    /******************************************/
    /*************** 버프 관리 ***************/
    /******************************************/

    #region Control Buff
    public virtual void AddBuffs(TransferConditionData _buffData)
    {
        if (overlapBuffGroup.ContainsKey(_buffData.GetBuffStatType))
        {
            // 중복이면 최신꺼로 대체하자
            int index = overlapBuffGroup[_buffData.GetBuffStatType];
            currentBuffs[index] = _buffData;
            return;
        }
        overlapBuffGroup.Add(_buffData.GetBuffStatType, currentBuffs.Count);
        currentBuffs.Add(_buffData);
    }

    public virtual void UpdateBuffs()
    {
        // 현재 스탯을 보내 다시 값을 계산하여 기존값과의 차이만큼 더해준다.
    }

    public virtual void FixedExecute()
    {
        // To Do ~~~~~
        // 버프 시간 체크
        if (buffCnt != 0)
        {
            // 뒤에서부터 체크하면 제거 후 생기는 재정렬에 의한 문제가 생기지 않음
            for (int i = buffCnt - 1; i >= 0; i--)
            {
                if (currentBuffs[i].IsMaintainBuff() == false)
                {
                    //currentBuffs[i].DeleteBuff();
                    overlapBuffGroup.Remove(currentBuffs[i].GetBuffStatType);
                    currentBuffs.RemoveAt(i);
                }
            }
        }
    }

    public virtual void RemoveAllBuffs()
    {

    }

    #endregion

    /******************************************/
    /********** 직접 스탯을 변경  **********/
    /******************************************/

    #region Virtual Stat Control Method 
    public virtual void TakeDamage(TransferAttackData _attackData) { statusUI.AnnounceChangeStat(); }
    public virtual void Heal(float _heal) { statusUI.AnnounceChangeStat(); }
    public virtual void Recovery(float _percent = 10f, float _time = 0.2f) { statusUI.AnnounceChangeStat(); }
    #endregion
}
