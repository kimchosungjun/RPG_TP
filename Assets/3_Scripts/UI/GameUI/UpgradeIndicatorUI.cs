using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIndicatorUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Levelup GradientFrame, 1:Direction, 2:Division")] Image[] images;
    [SerializeField] PlayerUpgradeIndicateSlot[] slots;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite slotFrame = res.GetSpriteAtlas("Bar_Atlas", "Gradient_Line");
        Sprite nextIcon = res.GetSpriteAtlas("Icon_Atlas", "Next_Icon");
        images[0].sprite = slotFrame;
        images[1].sprite = nextIcon;
        images[2].sprite = res.GetSpriteAtlas("Bar_Atlas", "Bot_Under_Division");

        int slotCnt = slots.Length;
        for(int i=0; i<slotCnt; i++)
        {
            slots[i].Init(slotFrame, nextIcon);
        }
    }
}
