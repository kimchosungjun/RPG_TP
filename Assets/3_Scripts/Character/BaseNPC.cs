using UnityEngine;
using UtilEnums;
public class BaseNPC : BaseCharacter
{
    public override void SetCharacterType()
    {
        intLayer = (int)LAYERS.NPC;
        bitLayer = 1<<(int)LAYERS.NPC;
        characterTableType = TABLE_FOLDER_TYPES.NPC;
    }
}
