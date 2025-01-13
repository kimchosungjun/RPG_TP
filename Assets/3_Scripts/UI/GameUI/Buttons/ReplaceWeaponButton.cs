using UnityEngine;
using UnityEngine.UI;

public class ReplaceWeaponButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    public void Init()
    {
        if(buttonImage==null) buttonImage = GetComponent<Image>();
        buttonImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "StartGame_Frame");
    }

    public void PressButton()
    {

    }
}
