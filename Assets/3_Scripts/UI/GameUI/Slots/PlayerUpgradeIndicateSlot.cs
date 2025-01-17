using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeIndicateSlot : MonoBehaviour
{
    [SerializeField,Tooltip("0:Frame,1:Dir")] Image[] images;
    [SerializeField, Tooltip("0:Legacy, 1:Current, 2:Name")] Text[] texts; 
    public void Init(Sprite _frameSprite, Sprite _dirSprite)
    {
        SetImage(_frameSprite, _dirSprite);
    }

    public void SetImage(Sprite _frameSprite, Sprite _dirSprite)
    {
        images[0].sprite  = _frameSprite;
        images[1].sprite = _dirSprite;
        this.gameObject.SetActive(false);
    }

    public void SetText(string _legacy, string _current, string _name)
    {
        texts[0].text = _legacy;
        texts[1].text = _current;
        texts[2].text = _name;
        this.gameObject.SetActive(true);
    }

    public void SetText()
    {
        texts[0].text = string.Empty;
        texts[1].text = string.Empty;
        texts[2].text = string.Empty;
        this.gameObject.SetActive(false);
    }
}
