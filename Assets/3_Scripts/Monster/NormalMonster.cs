using MonsterEnums;
using UnityEngine;

public class NormalMonster : BaseMonster
{
    [SerializeField] protected NormalMonsterStatusUI statusUI = null;

    public override void Recovery(float _percent = 10f, float _time = 0.2f) { monsterStatControl.Recovery(10f); }


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
    public override BTS IdleMovement() { return BTS.SUCCESS; }
    public override BTS DetectPlayer() { return BTS.SUCCESS; }
    protected override void CreateBTStates() { }
    #endregion
}
