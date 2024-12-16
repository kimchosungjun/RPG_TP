using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStatControl : MonoBehaviour
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
    public abstract void TakeDamage(TransferAttackData _attackData);
    public abstract void Heal(float _heal);
    public abstract void Recovery(float _percent);
    #endregion
}


/*
 1. 공격 활성화
 2. 현재 공격을 받을 수 있는 상태인지 확인해야 함 => 죽은 상태일수도, 이미 맞은 상태일수도, 무적상태일수도 있음. 
 3. 데미지를 받으면 데미지 받은 상태임을 움직임 제어하는 코드에서도 알아야 한다.
 
결론 : 한번에 정보를 전달이 필요하다.
=> 방안1.
1) BaseActor를 다시 생성하여 CanTakeDamage 메서드를 작성한다.
2) CanTakeDamage가 True면 StatControl과 행동제어 코드에 데미지와 상태를 보낸다. 
 
 */