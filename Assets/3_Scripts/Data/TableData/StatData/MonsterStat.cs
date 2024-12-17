using UnityEngine;

/// <summary>
/// 비전투 몬스터의 경우 바로 상속받아 사용한다.
/// </summary>
[System.Serializable]
public class MonsterStat : BaseStat
{
    #region Protected_StatValue
    [SerializeField] protected int monsterID;
    [SerializeField] protected float monsterCurHP;
    [SerializeField] protected float boostSpeed;
   #endregion

    #region Property_StatValue
    public int GetMonsterID { get { return monsterID; } }
    public float GetMonsterCurHP { get { return monsterCurHP; } }
    public float GetBoostSpeed { get { return boostSpeed; } } 
    #endregion
}