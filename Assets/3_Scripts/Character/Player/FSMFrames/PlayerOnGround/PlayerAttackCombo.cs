using UnityEngine;

[System.Serializable]
public class PlayerAttackCombo 
{
    /******************************************/
    /***********  콤보 & 시간   **************/
    /**************   생성자   ****************/
    /******************************************/

    #region Combo Value & Creator

    int attackCombo;
    int maxAttackCombo;
    float currentTime;
    float resetComboTime;

    public PlayerAttackCombo()
    {
        attackCombo = 0;
        maxAttackCombo = 3;
        resetComboTime = 1f;
        currentTime = -1f;
    }

    public PlayerAttackCombo(int _maxAttackCombo, float _resetComboTime = 1f)
    {
        attackCombo = 0;
        maxAttackCombo = _maxAttackCombo-1;
        resetComboTime = _resetComboTime;
        currentTime = -1f;
    }

    #endregion


    /******************************************/
    /********** 콤보 수 반환   ***************/
    /************ & 시간초기화  *************/
    /******************************************/

    #region Get Combo Value & Set Combo Time
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
    #endregion
}
