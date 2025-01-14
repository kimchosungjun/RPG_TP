using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeIndicateSlot : MonoBehaviour
{
    [SerializeField,Tooltip("0:Frame,1:Dir")] Image[] images;
    public void Init(Sprite _frameSprite, Sprite _dirSprite)
    {
        SetImage(_frameSprite, _dirSprite);
    }

    public void SetImage(Sprite _frameSprite, Sprite _dirSprite)
    {
        images[0].sprite  = _frameSprite;
        images[1].sprite = _dirSprite;  
    }
}
