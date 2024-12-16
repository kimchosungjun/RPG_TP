using UnityEngine;
using MonsterEnums;
using UnityEditorInternal;
using UnityEngine.UIElements;

public abstract class BaseMonster : BaseActor
{
    #region Value : Animator, StatControl
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatControl monsterStatControl;
    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Virtual
    protected virtual void Awake() { SetCharacterType(); if (monsterStatControl == null) monsterStatControl = GetComponent<MonsterStatControl>(); }
    protected virtual void Start() { CreateBTStates(); }
    protected virtual void FixedUpdate() { }
    #endregion

    #region Override : Set Layer
    public override void SetCharacterType() 
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    //   To Do ~~~~~~~
    public override bool CanTakeDamageState() { return true; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) {  }

    #endregion

    #region Abstract Method : To Do ~~~~~~~
    /// <summary>
    /// 플레이어가 근처에 오면 상태창 활성화
    /// </summary>
    /// <returns></returns>
    protected abstract void CreateBTStates();

    public abstract BTS DetectPlayer();
    public abstract BTS IdleMovement();
    public abstract BTS DetectMovement();

    public abstract void Spawn();
    public abstract void Death();
    public abstract void TakeDamage();
    public abstract void Recovery();
    #endregion
}