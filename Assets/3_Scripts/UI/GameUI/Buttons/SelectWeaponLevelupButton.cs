using UnityEngine;
using UnityEngine.UI;

public class SelectWeaponLevelupButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    public void Init()
    {
        if (buttonImage == null) buttonImage = GetComponent<Image>();
        buttonImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas_2", "Red_Long_Bar");
    }

    public void PressButton()
    {

    }
}
