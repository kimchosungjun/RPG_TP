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
    protected Dictionary<CONDITION_EFFECT_STATS, int> overlapBuffGroup = new Dictionary<CONDITION_EFFECT_STATS, int>();
    protected List<TransferConditionData> currentConditions = new List<TransferConditionData>();
    protected StatusUI statusUI = null;
    public void SetStatusUI(StatusUI _statusUI) { this.statusUI = _statusUI; }
    #endregion

    /******************************************/
    /*************** 버프 관리 ***************/
    /******************************************/

    #region Control Buff
    // Get Condition Data : MySelf (Direct)
    public virtual void AddCondition(TransferConditionData _conditionData)
    { 
        if (overlapBuffGroup.ContainsKey(_conditionData.GetConditionStat))
        {
            // Overlap => Replace Recent Condition Data
            int index = overlapBuffGroup[_conditionData.GetConditionStat];
            DeleteConditionData(currentConditions[index]);
            currentConditions[index] = _conditionData;
            ApplyConditionData(_conditionData);
            return;
        }
        overlapBuffGroup.Add(_conditionData.GetConditionStat, currentConditions.Count);
        currentConditions.Add(_conditionData);
        ApplyConditionData(_conditionData);
        buffCnt = currentConditions.Count;
    }

    public abstract void ApplyConditionData(TransferConditionData _conditionData);
    public abstract void DeleteConditionData(TransferConditionData _conditionData);
    public virtual void FixedExecute()
    {
        if (buffCnt != 0)
        {
            // Check Back to Front (Cause : Empty Index)
            for (int i = buffCnt - 1; i >= 0; i--)
            {
                currentConditions[i].UpdateConditionTime();
                if (currentConditions[i].GetIsEndConditionTime)
                {
                    DeleteConditionData(currentConditions[i]);
                    overlapBuffGroup.Remove(currentConditions[i].GetConditionStat);
                    currentConditions.RemoveAt(i);
                }
            }
        }
    }

    public virtual void RemoveAllConditionDatas()
    {
        int buffCnt = currentConditions.Count;
        if (buffCnt != 0)
        {
            for(int i= buffCnt-1; i>=0; i--)
            {
                DeleteConditionData(currentConditions[i]);
            }
            currentConditions.Clear();
            overlapBuffGroup.Clear();
        }
    }

    #endregion

    /******************************************/
    /********** 직접 스탯을 변경  **********/
    /********** 객체의 생존 여부  **********/
    /******************************************/

    #region Virtual Stat Control Method : Relate Entity Alive State
    public virtual void Death() { }
    public virtual void TakeDamage(TransferAttackData _attackData) { statusUI.AnnounceChangeStat(); }
    public virtual void Heal(float _heal) { statusUI.AnnounceChangeStat(); }
    public virtual void Recovery(float _percent = 10f, float _time = 0.2f) { statusUI.AnnounceChangeStat(); }
    #endregion

    
}
