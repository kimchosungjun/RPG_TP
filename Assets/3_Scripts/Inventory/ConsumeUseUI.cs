using UnityEngine;
using UnityEngine.UI;

public class ConsumeUseUI : MonoBehaviour
{
    ConsumeData consumeData = null;
    [SerializeField] GameObject consumeObject;
    [SerializeField] Image[] btnImages;

    public void Init()
    {
        SetImages();
    }

    public bool IsActive()
    {
        return consumeObject.activeSelf;
    }

    public void SetImages()
    {
        Sprite sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Red_Frame");
        btnImages[0].sprite = sprite;
        btnImages[1].sprite = sprite;
    }

    public void Active(ConsumeData _data)
    {
        consumeData = _data;
        consumeObject.SetActive(true);
    }

    public void InActive()
    {
        consumeData = null;
        consumeObject.SetActive(false);
    }

    public void Use()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        consumeData.Use();
        InActive();
    }

    public void NotUse()
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        InActive();
    }
}
