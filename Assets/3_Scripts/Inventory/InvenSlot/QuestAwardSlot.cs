using UnityEngine;
using UnityEngine.UI;

public class QuestAwardSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Icon, 1:Frame")] Image[] slotImages;
    [SerializeField] Text cntText;

    public void Init()
    {
        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "InvenSlot_Frame");
    }

    public void UpdateData()
    {

    }
}
