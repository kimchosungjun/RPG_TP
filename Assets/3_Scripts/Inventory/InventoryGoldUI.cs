using UnityEngine;
using UnityEngine.UI;

public class InventoryGoldUI : MonoBehaviour
{
    [SerializeField, Tooltip("0:Frame,1:Back")] Image[] goldImages;
    [SerializeField] Text goldText;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        goldImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Gold_Bar");
        goldImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_2", "Gold_Icon");
        goldText.text = SharedMgr.InventoryMgr.GetGold.ToString();
    }

    public void UpdateGold()
    {
        goldText.text = SharedMgr.InventoryMgr.GetGold.ToString();
    }
}
