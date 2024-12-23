using MonsterEnums;
using UnityEngine;

public class StandardMonster : BaseMonster
{
    // 스탯은 하위 클래스에서 생성
    [Header("스탯 UI"),SerializeField] protected StandardMonsterStatusUI statusUI = null;
    [SerializeField] protected MonsterStat monsterStat = null;
    public MonsterStat MonsterStat { get { return monsterStat; } set { monsterStat = value; } }
    /******************************************/
    /************ 재정의 메서드  ************/
    /******************************************/

    // Setup을 제외한 StatusUI 라이프 사이클 메서드 호출
    #region Override Life Cycle
    protected override void Awake()
    {
        base.Awake();
        statusUI.Init();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        statusUI.FixedExecute();
    }
    #endregion

    // 추후에 할 것
    #region Empty Override Metohd 
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { monsterStatControl.TakeDamage(_attackData);  statusUI.AnnounceChangeStat(UIEnums.STATUS.HP); }
    public override void Recovery(float _percent = 10f, float _time = 0.2f) { monsterStatControl.Recovery(10f); }
    public override NODESTATES IdleMovement() { return NODESTATES.SUCCESS; }
    protected override void CreateBTStates() { }
    #endregion
}
