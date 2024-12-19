
public abstract class BuffAction : CharacterAction
{
    protected ActorStatControl statCtrl;
    public ActorStatControl StatCtrl { get { return statCtrl; } }
    public override void DoAction() { DoBuff(); }
    public override void StopAction() { StopBuff(); }
    public abstract void DoBuff();
    public abstract void StopBuff();
}
