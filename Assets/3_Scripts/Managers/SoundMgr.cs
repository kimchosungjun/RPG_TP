using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UtilEnums;

public partial class SoundMgr : MonoBehaviour
{
    [SerializeField, Range(0, 5f)] float fadeBGMTime = 2f;  
    [SerializeField] AudioMixer audioMixer;
    AudioSource bgmSource;
    AudioSource sfxSource;
    Coroutine currentFade = null;
    BGMCLIPS currentBGM = BGMCLIPS.NONE;

    float masterVolume = 0f;
    float bgmVolume = 0f;
    float sfxVolume = 0;

    #region Set Sound Mgr Setting
    public void Init()
    {
        if (bgmSource==null) bgmSource = transform.Find("BGM").GetComponent<AudioSource>();  
        if (sfxSource == null) sfxSource = transform.Find("SFX").GetComponent<AudioSource>();
        SharedMgr.SoundMgr = this;
    }

    public void Setup()
    {
        SetSourceSetting();
        LoadSoundInfomation();
    }

    public void LoadSoundInfomation()
    {
        masterVolume = SharedMgr.SaveMgr.Option.masterVolume;
        bgmVolume = SharedMgr.SaveMgr.Option.bgmVolume;
        sfxVolume = SharedMgr.SaveMgr.Option.sfxVolume;
        ControlSound(SOUNDS.MASTER, masterVolume);
        ControlSound(SOUNDS.BGM, bgmVolume);
        ControlSound(SOUNDS.SFX, sfxVolume);
    }

    public void SetSourceSetting()
    {
        bgmSource.loop = true;
        sfxSource.loop = false;
        bgmSource.spatialBlend = 0;
        sfxSource.spatialBlend = 0;
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }
    #endregion

    #region Play BGM & SFX

    public void PlayBGM(BGMCLIPS _bgmClip, bool _isGameScene = true)
    {
        currentBGM = _bgmClip;
        if (bgmClipGroup.ContainsKey(_bgmClip) == false)
        {
            string path = "Sounds/BGM/" + Enums.GetEnumString<BGMCLIPS>(_bgmClip);
            Object obj = Resources.Load(path);
            if (obj == null)    // obj = null 이 성립되기 때문에 실수로 지우면 사운드 플레이 버그가 발생하지만 찾기가 어렵다.
                return;
            AudioClip clip = obj as AudioClip;
            if (null == clip)
                return;
            bgmClipGroup.Add(_bgmClip, clip);
        }

        if (_isGameScene)
        {
            if (currentFade != null)
                StopCoroutine(currentFade);
            currentFade = StartCoroutine(CFadeBGM());
        }
        else
        {
            bgmSource.Stop();
            bgmSource.clip = bgmClipGroup[currentBGM];
            bgmSource.volume = 1;
            bgmSource.Play();
        }
    }

    IEnumerator CFadeBGM()
    {
        float time = 0f;
        while (time < fadeBGMTime && bgmSource.volume > 0.01f)
        {
            time += Time.fixedDeltaTime;
            bgmSource.volume -= 1/fadeBGMTime * Time.fixedDeltaTime;    
            yield return new WaitForFixedUpdate();
        }

        if (bgmClipGroup.ContainsKey(currentBGM) ==false)
        {
            bgmSource.Stop();   
            bgmSource.clip = null;
            bgmSource.volume = 1;
            yield break;
        }

        bgmSource.Stop();
        bgmSource.volume = 0;
        bgmSource.clip = bgmClipGroup[currentBGM];
        bgmSource.Play();
        time = 0f;

        while (time < fadeBGMTime)
        {
            time += Time.fixedDeltaTime;
            bgmSource.volume += 1 / fadeBGMTime * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        bgmSource.volume = 1;
        currentFade = null;
    }

    public void PlaySFX(SFXCLIPS _sfxClip)
    {
        if (sfxClipGroup.ContainsKey(_sfxClip) == false)
        {
            string path = "Sounds/SFX/" + Enums.GetEnumString<SFXCLIPS>(_sfxClip);
            Object obj = Resources.Load(path);
            if (obj == null)    
                return;
            AudioClip clip = obj as AudioClip;
            if (clip == null)
                return;
            sfxClipGroup.Add(_sfxClip, clip);
        }

        sfxSource.PlayOneShot(sfxClipGroup[_sfxClip]);
    }

    public void PressButtonSFX() { PlaySFX(SFXCLIPS.BUTTON_SFX); }
    #endregion

    #region Control Sound

    public void ControlSound(SOUNDS _soundType, float _value)
    {
        float setValue = Mathf.Log10(_value) * 20;
        switch (_soundType)
        {
            case SOUNDS.MASTER:
                masterVolume = _value;
                SharedMgr.SaveMgr.Option.masterVolume = _value;
                audioMixer.SetFloat("Master", setValue);
                break;
            case SOUNDS.BGM:
                bgmVolume = _value;
                SharedMgr.SaveMgr.Option.bgmVolume = _value;
                audioMixer.SetFloat("BGM", setValue);
                break;
            case SOUNDS.SFX:
                sfxVolume = _value;
                SharedMgr.SaveMgr.Option.sfxVolume = _value;
                audioMixer.SetFloat("SFX", setValue);
                break;
        }
    }

    public float GetSoundVolume(SOUNDS _soundType)
    {
        switch (_soundType)
        {
            case SOUNDS.MASTER:
                return masterVolume;
            case SOUNDS.BGM:
                return bgmVolume;
            case SOUNDS.SFX:
                return sfxVolume;
        }
        return masterVolume;
    }

    #endregion
}
