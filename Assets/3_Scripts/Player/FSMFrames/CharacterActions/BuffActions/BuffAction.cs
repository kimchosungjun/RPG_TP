
public abstract class BuffAction : CharacterAction
{
    protected CharacterStatCtrl statCtrl;
    public CharacterStatCtrl StatCtrl { get { return statCtrl; } }
    public override void DoAction() { DoBuff(); }
    public override void StopAction() { StopBuff(); }
    public abstract void DoBuff();
    public abstract void StopBuff();
}
