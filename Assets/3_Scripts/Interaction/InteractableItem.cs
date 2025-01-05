using UnityEngine;

public abstract class InteractableItem : Interactable
{
    [SerializeField] protected ItemEnums.ITEMID itemID;

    protected string getString = " 줍기";
    public override string Detect() { return (GetItemName() + getString); }
    public abstract string GetItemName();

    public override void Interact() { GetItem(); }
    public abstract void GetItem();

    public abstract void LoadItemData();
}
