using UnityEngine;

public abstract class BuffAction : MonoBehaviour
{
    protected ActorStatControl statCtrl;
    public ActorStatControl StatCtrl { get { return statCtrl; } }
    public abstract void DoBuff();
    public abstract void StopBuff();
}
