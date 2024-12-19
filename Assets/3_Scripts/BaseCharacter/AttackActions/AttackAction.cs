public abstract class AttackAction : CharacterAction
{
    public override void DoAction() { DoAttack(); }
    public override void StopAction() { StopAttack(); }
    public abstract void DoAttack();
    public abstract void StopAttack();
}
