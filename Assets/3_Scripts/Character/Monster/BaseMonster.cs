using UnityEngine;
using MonsterEnums;
using System;
using System.Collections;

[RequireComponent(typeof(MonsterStatControl))]
public abstract class BaseMonster : BaseActor
{
    /******************************************/
    /************ 연결 컴포넌트  ************/
    /********** 시야 감지 & 영역  ***********/
    /******************************************/

    #region Value : Animator, StatControl
    [Header("컴포넌트"), SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatControl monsterStatControl;
    protected MonsterStat monsterStat;
    #endregion

    #region Value : Notice
    protected int playerLayer = 1 << 8;
    protected PathNode[] pathNodes;
    protected bool isInMonsterArea = false;
    public BattleField MonsterArea { protected get; set; }
    #endregion

    #region Value : Monster Information
    [SerializeField] protected InitMonsterData initMonsterData;
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
    public override void ApplyCondition(TransferConditionData _conditionData) { monsterStatControl.AddCondition(_conditionData); }
    #endregion

    /******************************************/
    /************ 라이프 사이클  ************/
    /******************************************/

    #region Virtual : Life Cycle
    /// <summary>
    /// Set Character Type, Link Monster Stat
    /// </summary>
    protected virtual void Awake()
    {
        SetCharacterType();
        if (monsterStatControl == null)
            monsterStatControl = GetComponent<MonsterStatControl>();
        monsterStatControl.BaseMonster = this;
    }

    /// <summary>
    /// Create BT States
    /// </summary>
    protected virtual void Start() 
    { 
        CreateBTStates();
        MonsterTable table = SharedMgr.TableMgr.GetMonster;
        MonsterTableClassGroup.MonsterInfoTableData infoTableData = table.GetMonsterInfoTableData(initMonsterData.monsterType);
        MonsterTableClassGroup.MonsterStatTableData statTableData = table.GetMonsterStatTableData(initMonsterData.monsterType);
        monsterStat.SetMonsterStat(statTableData, initMonsterData.monsterLevel);
        monsterStatControl.MonsterStat = monsterStat;
    }
    
    /// <summary>
    /// Empty
    /// </summary>
    protected virtual void FixedUpdate() { }
    #endregion

    /******************************************/
    /************* 가상 메서드  *************/
    /******************************************/

    #region Virtual Method : Announce Area & Spawn 
    public virtual void AnnounceInMonsterArea() { isInMonsterArea = true; } 
    public virtual void AnnounceOutMonsterArea() { isInMonsterArea = false; }
    public virtual void SetPathNodes(PathNode[] _pathNodes) { pathNodes = _pathNodes; } 
    public virtual void Spawn(Vector3 _spawnPosition) { this.transform.position = _spawnPosition; this.gameObject.SetActive(true); }
    public virtual void Death() { anim.SetInteger("MState", (int)STATES.DEATH); SetNoneInteractionType(); } // 애니메이션 설정하기
    public virtual void AfterDeath() { MonsterArea.DeathMonster(this.gameObject); this.gameObject.SetActive(false); } // 스탯 원래대로 만들기 추가 

    protected bool isRecovery = false;
    public virtual void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        if (isRecovery) return;
        StartCoroutine(CRecovery(_percent, _time));
    }
    protected virtual IEnumerator CRecovery(float _percent, float _time)
    {
        float time = 0f;
        while (true)
        {
            if (isRecovery == false) yield break;
            if(monsterStatControl.CheckFullHp()) { isRecovery = false; yield break; }
            time+= Time.fixedDeltaTime;
            if(time >= _time)
            {
                time = 0f;
                monsterStatControl.Heal(_percent / 100, true);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region Abstract Method 
    /// <summary>
    /// 몬스터에 맞는 BT 구조 생성하기
    /// </summary>
    protected abstract void CreateBTStates();

    /// <summary>
    /// 공격받은지 오래되거나 본래의 자리로 돌아갈 때 호출 : 자동 치유
    /// </summary>
  
    #endregion
}

[Serializable]
public class InitMonsterData
{
    public TYPEIDS monsterType;
    public int monsterLevel;
}