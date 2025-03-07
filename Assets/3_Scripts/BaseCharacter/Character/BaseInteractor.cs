using UnityEngine;
using UtilEnums;
public abstract class BaseInteractor : BaseCharacter
{
    [SerializeField] protected string detectText;
    public override void SetCharacterType()
    {
        intLayer = (int)LAYERS.INTERACTOR;
        bitLayer = 1<<(int)LAYERS.INTERACTOR;
        //characterTableType = TABLE_FOLDER_TYPES.NPC;
    }

    public abstract string Detect(ref string _interactionText);
    public abstract void Interact();
}
