using UnityEngine;
using UnityEngine.UI;

public class InventoryExitUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame,1:Back")] Image[] exitBtnImages;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        exitBtnImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("JoyStick_Atlas", "JoyStick_Frame");
        exitBtnImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Back_Icon");
    }
}
