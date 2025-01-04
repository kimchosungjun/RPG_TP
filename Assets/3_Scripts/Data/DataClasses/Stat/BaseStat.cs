using UnityEngine;

[System.Serializable]
public class BaseStat 
{
    #region Protected
    // 행위자의 식별 정보 : ID는 몬스터와 플레이어를 나눠서 따로 저장할 생각
    [SerializeField] protected string actorName;
    // 행위자의 공통 변수
    [SerializeField] protected int maxHp;
    [SerializeField] protected float speed;
    [SerializeField] protected int defenceValue;
    #endregion

    #region Public : Property
    // 행위자의 식별 정보
    public string GetActorName { get { return actorName; } }

    // 행위자의 공통 변수
    public int MaxHP { get { return maxHp; } set { maxHp = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public int Defence { get { return defenceValue; } set { defenceValue = value; } }
    #endregion
}
