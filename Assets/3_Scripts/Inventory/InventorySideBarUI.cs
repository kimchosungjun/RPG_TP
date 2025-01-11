using UnityEngine;
using UnityEngine.UI;

public class InventorySideBarUI : MonoBehaviour
{
    [SerializeField] SideBarSlot[] sideSlots; // 아틀라스로 이미지 설정
    [SerializeField] Image sideBarFrameImage; 
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        sideBarFrameImage.sprite = res.GetSpriteAtlas("Slot_Atlas", "SideSlot_Frame");
        sideSlots[0].SetImage(res.GetSpriteAtlas("Icon_Atlas_2", "Etc_Btn"));
        sideSlots[1].SetImage(res.GetSpriteAtlas("Icon_Atlas_2", "Consume_Btn"));
        sideSlots[2].SetImage(res.GetSpriteAtlas("Icon_Atlas_2", "Weapon_Btn"));
    }
}
