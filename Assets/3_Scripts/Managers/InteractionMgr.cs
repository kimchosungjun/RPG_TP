using System.Collections;
using System.Collections.Generic;

public partial class InteractionMgr
{
    HashSet<Interactable> interactableSet;

    public void Init()
    {
        SharedMgr.InteractionMgr = this;
        interactableSet = new HashSet<Interactable>();  
    }
    
    public void Setup()
    {
        LoadDialogues("Dialogues");
    }


    public void AddInteractable(Interactable _interactable)
    {
        if (interactableSet.Contains(_interactable)) return;
        interactableSet.Add(_interactable);
        SharedMgr.UIMgr.GameUICtrl.GetInteractionUI.AddInteractable(_interactable);
    }

    public void RemoveInteractable(Interactable _interactable)
    {
        if (interactableSet.Contains(_interactable) == false) return;
        interactableSet.Remove(_interactable);  
        SharedMgr.UIMgr.GameUICtrl.GetInteractionUI.RemoveInteractable(_interactable);
    }
}