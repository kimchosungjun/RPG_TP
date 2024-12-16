using UnityEngine;
using MonsterEnums;

public abstract class BaseMonster : BaseActor
{
    #region Value : Animator, StatControl
    protected int playerLayer = 1 << 8;
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatControl monsterStatControl;
    #endregion

    #region Value : Notice
    protected bool isInMonsterArea = false;
    public BattleField MonsterArea { protected get; set; }  
    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Virtual : Life Cycle
    protected virtual void Awake() { SetCharacterType(); if (monsterStatControl == null) monsterStatControl = GetComponent<MonsterStatControl>(); }
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

    //   To Do ~~~~~~~
    public override bool CanTakeDamageState() { return true; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { monsterStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) { }

    #endregion

    #region Virtual Method : Announce Area & Spawn 
    public virtual void AnnounceInMonsterArea() { isInMonsterArea = true; } 
    public virtual void AnnounceOutMonsterArea() { isInMonsterArea = false; }
    public virtual void Spawn(Vector3 _spawnPosition) { this.transform.position = _spawnPosition; this.gameObject.SetActive(true); }
    public virtual void Death() { /*anim.SetInteger("States", 9);*/ } // 애니메이션 설정하기
    public virtual void AfterDeath() { MonsterArea.DeathMonster(this.gameObject); this.gameObject.SetActive(false); } // 스탯 원래대로 만들기 추가 
    #endregion

    #region Abstract Method 

    /// <summary>
    /// Create BT Tree
    /// </summary>
    protected abstract void CreateBTStates();

    public abstract BTS DetectPlayer();
    public abstract BTS IdleMovement();
    public abstract void Recovery();
    #endregion
}