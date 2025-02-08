using UnityEngine;
using UnityEngine.UI;


public class SoundControlUI : MonoBehaviour, ITurnOnOffUI
{
    #region Variable
    [SerializeField] GameObject soundUIParent;

    [SerializeField, Tooltip("0:MasterFrame, 1:MasterBtn, 2:BGMFrame, 3:BgmBtn, 4:SFXFrame, 5:SFXBtn")] 
    Image[] soundUIImages;

    [SerializeField, Tooltip("0:MasterPercent, 1:BgmPercent, 2:SFXPercent")]
    Text[] soundPercentText;

    [SerializeField, Tooltip("0:Master,1:Bgm,2:SFX")] Slider[] soundSliders;
    #endregion

    #region Local Enum

    enum SoundImage
    {
        MasterFrame=0,
        MasterBtn=1,
        BgmFrame=2,
        BgmBtn=3,
        SfxFrame=4,
        SfxBtn=5,
        Exit=6
    }

    enum PercentText
    {
        Master=0,
        Bgm=1,
        SFX=2
    }

    #endregion

    #region Set UI
    public void Init()
    {
        SetImages();
        SetSoundValue();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        Sprite frameSpr = res.GetSpriteAtlas("Bar_Atlas", "Loading_Bar");
        Sprite btnSpr = res.GetSpriteAtlas("Icon_Atlas", "Red_Empty");
        soundUIImages[0].sprite = frameSpr;
        soundUIImages[1].sprite = btnSpr;
        soundUIImages[2].sprite = frameSpr;
        soundUIImages[3].sprite = btnSpr;
        soundUIImages[4].sprite = frameSpr;
        soundUIImages[5].sprite = btnSpr;
    }

    public void SetSoundValue()
    {
        // To Do Load Data
        soundSliders[0].maxValue= 1;
        soundSliders[1].maxValue = 1;
        soundSliders[2].maxValue = 1;
    }
    #endregion

    #region Turn On & Off
    public void TurnOn()
    {
        SoundMgr sound = SharedMgr.SoundMgr;
        soundSliders[0].value = sound.GetSoundVolume(UtilEnums.SOUNDS.MASTER);
        soundSliders[1].value = sound.GetSoundVolume(UtilEnums.SOUNDS.BGM);
        soundSliders[2].value = sound.GetSoundVolume(UtilEnums.SOUNDS.SFX);
        if (soundUIParent.activeSelf == false)
            soundUIParent.SetActive(true);
    }

    public void TurnOff()
    {
        if(soundUIParent.activeSelf)
            soundUIParent.SetActive(false);
        SoundMgr sound = SharedMgr.SoundMgr;
        sound.ControlSound(UtilEnums.SOUNDS.MASTER, soundSliders[0].value);
        sound.ControlSound(UtilEnums.SOUNDS.BGM, soundSliders[1].value);
        sound.ControlSound(UtilEnums.SOUNDS.SFX, soundSliders[2].value);
    }

    public void ApplySetting()
    {
        SoundMgr sound = SharedMgr.SoundMgr;
        sound.ControlSound(UtilEnums.SOUNDS.MASTER, soundSliders[0].value);
        sound.ControlSound(UtilEnums.SOUNDS.BGM, soundSliders[1].value);
        sound.ControlSound(UtilEnums.SOUNDS.SFX, soundSliders[2].value);
    }
    #endregion

    #region Value Change

    public void ChangeMasterValue(float _value)
    {
        soundSliders[0].value = _value;
    }


    public void ChangeBGMValue(float _value)
    {
        soundSliders[1].value = _value;
    }

    public void ChangeSFXValue(float _value)
    {
        soundSliders[2].value = _value;
    }
    #endregion
}
