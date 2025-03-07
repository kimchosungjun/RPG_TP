public abstract class BaseActor : BaseCharacter
{
    public override void SetCharacterType() {  /* 이를 상속받는 몬스터와 캐릭터는 다시 선언해야 함*/}

    /**************************************/
    /***********  피격 가능  *************/
    /********** 상태인지 확인 **********/
    /**************************************/
    #region Relate Take Damage
    public virtual void TakeDamage(TransferAttackData _attackData)
    {
        if (CanTakeDamageState() == false)
            return;
        ApplyMovementTakeDamage(_attackData);
        ApplyStatTakeDamage(_attackData);
    }
    public abstract bool CanTakeDamageState();
    public abstract void ApplyMovementTakeDamage(TransferAttackData _attackData);
    public abstract void ApplyStatTakeDamage(TransferAttackData _attackData);

    public virtual void TakeCondition(TransferConditionData _conditionData)
    {
        if (_conditionData == null) return;
        ApplyCondition(_conditionData);
    }

    public abstract void ApplyCondition(TransferConditionData _conditionData);
    #endregion
}
