using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    int index = 0;

    [SerializeField] Image slotImage;
    [SerializeField] Button slotBtn;
    [SerializeField] Text slotText;
    public void Init(int _index)
    {
        index = _index;
        SetImage();
    }

    public void SetImage()
    {
        slotImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Gradient_Line");
    }

    public void UpdateSlotData()
    {
        slotBtn.interactable = false;
        slotText.text = string.Empty;
    }

    public void UpdateSlotData(string _text)
    {
        slotBtn.interactable = true;
        slotText.text = (index + 1) + ". " + _text; 
    }

    public void PressSlot()
    {
        SharedMgr.UIMgr.GameUICtrl.GetQuestUI.PressQuestSlot(index);
    }
}
