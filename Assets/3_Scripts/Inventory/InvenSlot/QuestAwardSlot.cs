using UnityEngine;
using UnityEngine.UI;

public class QuestAwardSlot : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame, 1:Icon, 2:AmountFrame")] Image[] slotImages;
    [SerializeField] Text cntText;

    public void Init()
    {
        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "InvenSlot_Frame");
        slotImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas_2", "Delete_Bar");
    }

    public void UpdateData(Sprite _sprite, int _quantity)
    {
        slotImages[1].sprite = _sprite;
        cntText.text = _quantity.ToString();
        if(gameObject.activeSelf==false)
            this.gameObject.SetActive(true);
    }

    public void UpdateData(Sprite _sprite, string _description)
    {
        slotImages[1].sprite = _sprite;
        cntText.text = _description;
        if (gameObject.activeSelf == false)
            this.gameObject.SetActive(true);
    }


    public void UpdateData()
    {
        if (gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }
}
