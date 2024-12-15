using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStatCtrl : MonoBehaviour
{
    protected int buffCnt = 0;
    protected List<TransferBuffData> currentBuffs = new List<TransferBuffData>();

    #region Control Buff
    public virtual void AddBuffs(TransferBuffData _buffData)
    {
        if (currentBuffs.Contains(_buffData))
        {
            _buffData.OverlapBuff();
            return;
        }
        _buffData.AddBuff();
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
                    currentBuffs[i].DeleteBuff();
                    currentBuffs.RemoveAt(i);
                }
            }
        }
    }
    #endregion

    #region Abstract
    public abstract void TakeDamage(float _damage);
    public abstract void Heal(float _heal);
    public abstract void Recovery(float _percent);
    #endregion
}
