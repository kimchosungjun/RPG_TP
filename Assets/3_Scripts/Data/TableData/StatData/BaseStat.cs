using UnityEngine;

[System.Serializable]
public class BaseStat 
{
    #region Protected
    // 행위자의 식별 정보 : ID는 몬스터와 플레이어를 나눠서 따로 저장할 생각
    [SerializeField] protected string actorName;
    
    // 행위자의 공통 변수
    [SerializeField] protected float maxHp;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackValue;
    [SerializeField] protected float defenceValue;
    [SerializeField] protected float criticalValue;
    [SerializeField] protected float attackSpeed; // 공격속도는 애니메이션 속도에 영향을 준다.
    [SerializeField] protected int level;
    #endregion

    #region Public : Property
    // 행위자의 식별 정보
    public string ActorName { get { return actorName; } set { actorName = value; } }

    // 행위자의 공통 변수
    public float HP { get { return maxHp; } set { maxHp = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Attack { get {  return attackValue; } set { attackValue = value; } }
    public float Defence { get { return defenceValue; } set { defenceValue = value; } }
    public float Critical { get { return criticalValue; } set { criticalValue = value; } }
    public float AttackSpeed { get { return attackSpeed;  } set { attackSpeed = value; } } 
    public int Level { get { return level; } set { level = value; } }
    #endregion
}
