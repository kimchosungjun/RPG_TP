using MonsterEnums;
using UnityEngine;

public class StandardMonster : BaseMonster
{
    // 스탯은 하위 클래스에서 생성

    [Header("스탯 UI"),SerializeField] protected StandardMonsterStatusUI statusUI = null;
    
    /******************************************/
    /************ 재정의 메서드  ************/
    /******************************************/

    #region Override Life Cycle
    protected override void Awake()
    {
        base.Awake();
        statusUI.Init();
        // 스탯 불러와서 넣기
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

    #region Empty Override Metohd 
    public override void Recovery(float _percent = 10f, float _time = 0.2f) { monsterStatControl.Recovery(10f); }
    public override BTS IdleMovement() { return BTS.SUCCESS; }
    public override BTS DetectPlayer() { return BTS.SUCCESS; }
    protected override void CreateBTStates() { }
    #endregion
}
