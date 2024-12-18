using UnityEngine;
using MonsterEnums;

[RequireComponent(typeof(MonsterStatControl))]
public abstract class BaseMonster : BaseActor
{
    /******************************************/
    /************ 연결 컴포넌트  ************/
    /********** 시야 감지 & 영역  ***********/
    /******************************************/

    #region Value : Animator, StatControl
    [Header("컴포넌트"),SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatControl monsterStatControl;
    #endregion

    #region Value : Notice
    protected int playerLayer = 1 << 8;
    protected PathNode[] pathNodes;
    protected bool isInMonsterArea = false;
    public BattleField MonsterArea { protected get; set; }
    #endregion

    #region Value : Monster Information
    [Header("몬스터의 정보"),SerializeField, Tooltip("몬스터의 스탯을 불러오기 위해 필요")] protected TYPEIDS monsterType;
    [SerializeField, Tooltip("CSV 파일에서 저장된 레벨 인덱스로 레벨 정보를 불러오는 역할")] protected int monsterLevelIndex;
    [SerializeField, Tooltip("현재는 인덱스로 불러오기 때문에 모든 기능이 완성되면 인스펙터에서 보이지 않게 하기")] protected int monsterLevel;
    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Virtual : Life Cycle
    protected virtual void Awake() 
    {
        SetCharacterType(); 
        if (monsterStatControl == null) 
            monsterStatControl = GetComponent<MonsterStatControl>();
        monsterStatControl?.SetBaseMonster(this);
    }
    protected virtual void Start() { CreateBTStates(); }
    protected virtual void FixedUpdate() { }
    #endregion

    /******************************************/
    /************* 레이어 설정  *************/
    /************* 피격 메서드  *************/
    /******************************************/

    #region Override : Set Layer & Take Damage
    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    public override bool CanTakeDamageState() { return true; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { monsterStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) { }

    #endregion

    #region Virtual Method : Announce Area & Spawn 
    public virtual void AnnounceInMonsterArea() { isInMonsterArea = true; } 
    public virtual void AnnounceOutMonsterArea() { isInMonsterArea = false; }
    public virtual void SetPathNodes(PathNode[] _pathNodes) { pathNodes = _pathNodes; } 
    public virtual void Spawn(Vector3 _spawnPosition) { this.transform.position = _spawnPosition; this.gameObject.SetActive(true); }
    public virtual void Death() { anim.SetInteger("MState", (int)STATES.DEATH); SetNoneInteractionType(); } // 애니메이션 설정하기
    public virtual void AfterDeath() { MonsterArea.DeathMonster(this.gameObject); this.gameObject.SetActive(false); } // 스탯 원래대로 만들기 추가 
    #endregion

    #region Abstract Method 
    /// <summary>
    /// 몬스터에 맞는 BT 구조 생성하기
    /// </summary>
    protected abstract void CreateBTStates();
    /// <summary>
    /// 정해진 Pathnode에서 돌아다니거나 가만히 있거나.. 등 평상시의 AI
    /// </summary>
    /// <returns></returns>
    public abstract NODESTATES IdleMovement();
    /// <summary>
    /// 공격받은지 오래되거나 본래의 자리로 돌아갈 때 호출 : 자동 치유
    /// </summary>
    public abstract void Recovery(float _percent = 10f, float _time = 0.2f);
    #endregion
}