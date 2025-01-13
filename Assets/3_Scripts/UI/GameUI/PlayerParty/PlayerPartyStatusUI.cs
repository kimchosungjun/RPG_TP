using UnityEngine;
using UnityEngine.UI;

public class PlayerPartyStatusUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:HP, 1:Atk, 2:Def, 3:Cri, 4:PlayerIcon")] Image[] icons;
    [SerializeField, Tooltip("0:Hp, 1:Atk, 2:Def, 3:Cir, 4:Player")] Image[] iconFrames;
    [SerializeField] PlayerPartyStatusButton[] buttons;

    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        icons[0].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "HP_Icon");
        icons[1].sprite = res.GetSpriteAtlas("Icon_Atlas", "Weapon_Icon");
        icons[2].sprite = res.GetSpriteAtlas("Icon_Atlas_4", "Defence_Icon");
        icons[3].sprite = res.GetSpriteAtlas("Icon_Atlas", "Critical_Icon");

        Sprite slotSprite = res.GetSpriteAtlas("Bar_Atlas_2", "Delete_Bar");
        iconFrames[0].sprite = slotSprite;
        iconFrames[1].sprite = slotSprite;
        iconFrames[2].sprite = slotSprite;
        iconFrames[3].sprite = slotSprite;
        iconFrames[4].sprite = res.GetSpriteAtlas("Bar_Atlas_3", "PartyPlayerStatus_Frame");

        int slotCnt = buttons.Length;
        for (int i=0; i <slotCnt; i++)
        {
            buttons[i].Init();
        }
    }

    public void PressCharacter(int _id)
    {

    }
}
