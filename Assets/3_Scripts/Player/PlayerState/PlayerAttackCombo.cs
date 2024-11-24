using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAttackCombo 
{
    int attackCombo;
    int maxAttackCombo;

    float currentTime;
    float resetComboTime;

    public PlayerAttackCombo()
    {
        attackCombo = 0;
        maxAttackCombo = 3;
        currentTime = -1f;
    }

    public PlayerAttackCombo(int _maxAttackCombo)
    {
        attackCombo = 0;
        maxAttackCombo = _maxAttackCombo;
        currentTime = -1f;
    }

    /// <summary>
    /// 현재 콤보 수를 체크 : 0 ~ Maxcombo-1까지
    /// </summary>
    /// <returns></returns>
    public int GetCombo()
    {
        float inputTime = Time.time;
        if(inputTime - currentTime > resetComboTime)
        {
            attackCombo = 0;
        }
        else
        {
            attackCombo += 1;
            if (attackCombo >= maxAttackCombo)
                attackCombo = 0;
        }
        //currentTime = inputTime;
        return attackCombo;
    }
    
    /// <summary>
    /// 애니메이션이 끝나는 시간을 기준으로 콤보 시간 체크
    /// </summary>
    public void SetComboTime() { currentTime = Time.time; }
}
