using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UtilEnums;


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
        soundSliders[0].minValue= 0.01f;
        soundSliders[1].minValue = 0.01f;
        soundSliders[2].minValue = 0.01f;

        soundSliders[0].maxValue= 1;
        soundSliders[1].maxValue = 1;
        soundSliders[2].maxValue = 1;

        soundSliders[0].value = SharedMgr.SoundMgr.GetSoundVolume(SOUNDS.MASTER);
        soundSliders[1].value = SharedMgr.SoundMgr.GetSoundVolume(SOUNDS.BGM);
        soundSliders[2].value = SharedMgr.SoundMgr.GetSoundVolume(SOUNDS.SFX);

        ChangeIndicatePercent(SOUNDS.MASTER);
        ChangeIndicatePercent(SOUNDS.BGM);
        ChangeIndicatePercent(SOUNDS.SFX);


        soundSliders[0].onValueChanged.RemoveAllListeners();
        soundSliders[0].onValueChanged.AddListener(ChangeMasterValue);
        soundSliders[1].onValueChanged.RemoveAllListeners();
        soundSliders[1].onValueChanged.AddListener(ChangeBGMValue);
        soundSliders[2].onValueChanged.RemoveAllListeners();
        soundSliders[2].onValueChanged.AddListener(ChangeSFXValue);
    }
    #endregion

    #region Turn On & Off
    public void TurnOn()
    {
        if (soundUIParent.activeSelf == false)
            soundUIParent.SetActive(true);
    }

    public void TurnOff()
    {
        if(soundUIParent.activeSelf)
            soundUIParent.SetActive(false);
    }
    #endregion

    #region Value Change

    public void ChangeMasterValue(float _value)
    {
        soundSliders[0].value = _value;
        SharedMgr.SoundMgr.ControlSound(SOUNDS.MASTER, _value);
        ChangeIndicatePercent(SOUNDS.MASTER);
    }


    public void ChangeBGMValue(float _value)
    {
        soundSliders[1].value = _value;
        SharedMgr.SoundMgr.ControlSound(SOUNDS.BGM, _value);
        ChangeIndicatePercent(SOUNDS.BGM);
    }

    public void ChangeSFXValue(float _value)
    {
        soundSliders[2].value = _value;
        SharedMgr.SoundMgr.ControlSound(SOUNDS.SFX, _value);
        ChangeIndicatePercent(SOUNDS.SFX);
    }

    public void ChangeIndicatePercent(SOUNDS _soundType)
    {
        int index = (int)_soundType;
        soundPercentText[index].text = (int)((soundSliders[index].value * 100)) + "%";
    }
    #endregion
}
