using UnityEngine;

/// <summary>
/// 파일로부터 불러와 사용하는 스탯 데이터 : 고정 데이터
/// </summary>
public class PlayerStat : BaseStat
{
    #region Protected
    // 플레이어 고유 스탯
    [SerializeField] protected float dashSpeed;
    [SerializeField] protected float jumpSpeed;
    [SerializeField] protected float attackSpeed;
    #endregion

    #region Public
    // 플레이어 고유 스탯
    public float DashSpeed { get { return dashSpeed; } set { dashSpeed = value; } }
    public float JumpSpeed { get { return jumpSpeed; } set { jumpSpeed = value; } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }   
    #endregion

    #region Current Stat
    // 현재 캐릭터의 정보를 담은 정보 : 저장해야 하는 데이터
    protected PlayerCurrentStat currentStat = null;
    protected PlayerCurrentStat CurrentStat { get { return currentStat; } }
    public void SetPlayerCurrentStat(PlayerCurrentStat _currentStat) { this.currentStat = _currentStat; }
    #endregion
}


/// <summary>
/// 저장하고 저장한 데이터를 불러와 사용하는 스탯 데이터 : 가변 데이터
/// </summary>
[System.Serializable]
public class PlayerCurrentStat
{
    [SerializeField] protected float currentHP;
    [SerializeField] protected int currentExp;
    [SerializeField] protected int currentNormalAttackLevel;
    [SerializeField] protected int currentSkillLevel;
    [SerializeField] protected int currentUltimateSkillLevel;

    
    public int CurrentUltimateSkillLevel { get { return currentUltimateSkillLevel; }}

    public bool CanLevelUp(E_ENHANCE_PLAYER _enhanceStat) 
    {

        return true; 
    }
}

