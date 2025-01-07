using UnityEngine;
public abstract class Interactable : MonoBehaviour
{
    public abstract string Detect();
    public abstract void Interact();

    public virtual void ChangeToDisable() 
    {
        this.gameObject.layer = (int)UtilEnums.LAYERS.UNINTERACTOR;
        this.enabled = false;
    }
}