using UnityEngine;

[System.Serializable]
public class BaseStat 
{
    #region Protected
    // 행위자의 식별 정보
    [SerializeField] protected int id;
    [SerializeField] protected string actorName;

    // 행위자의 공통 변수
    [SerializeField] protected float hp;
    [SerializeField] protected float speed;
    [SerializeField] protected float attack;
    [SerializeField] protected float defence;
    [SerializeField] protected float critical;
    [SerializeField] protected int level;
    #endregion

    #region Public : Property
    // 행위자의 식별 정보
    public int ID { get { return id;} set { id = value; } }
    public string ActorName { get { return actorName; } set { actorName = value; } }

    // 행위자의 공통 변수
    public float HP { get { return hp; } set { hp = value; } }
    public float Speed { get { return speed; } set { speed = value; } }
    public float Attack { get {  return attack; } set { attack = value; } }
    public float Defence { get { return defence; } set { defence = value; } }
    public float Critical { get { return critical; } set { critical = value; } }
    public int Level { get { return level; } set { level = value; } }
    #endregion
}
