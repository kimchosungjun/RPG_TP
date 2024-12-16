
public abstract class BuffAction : CharacterAction
{
    protected CharacterStatControl statCtrl;
    public CharacterStatControl StatCtrl { get { return statCtrl; } }
    public override void DoAction() { DoBuff(); }
    public override void StopAction() { StopBuff(); }
    public abstract void DoBuff();
    public abstract void StopBuff();
}
