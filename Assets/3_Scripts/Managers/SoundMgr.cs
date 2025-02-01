using UnityEngine;
using UnityEngine.Audio;

public class SoundMgr : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    AudioSource bgmSource;
    AudioSource sfxSource;

    float masterVolume = 0f;
    float bgmVolume = 0f;
    float sfxVolume = 0;

    public void Init()
    {
        if(bgmSource==null)bgmSource=transform.Find("BGM").GetComponent<AudioSource>();
        bgmSource.loop = true;
        if(sfxSource == null) sfxSource = transform.Find("Effect").GetComponent<AudioSource>();
        SharedMgr.SoundMgr = this;
    }

    public void Setup()
    {
        LoadSoundInfomation();
    }

    public void LoadSoundInfomation()
    {
        masterVolume = 0f;
        bgmVolume = 0f;
        sfxVolume = 0;
    }

    public void PlayBGM(string _bgm)
    {
        Object obj = Resources.Load(_bgm);
        if (obj == null)    // obj = null 이 성립되기 때문에 실수로 지우면 사운드 플레이 버그가 발생하지만 찾기가 어렵다.
            return;
        AudioClip clip = obj as AudioClip;
        if (null == clip)
            return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayerEffect(string _effect)
    {
        Object obj = Resources.Load(_effect); 
        if (null == obj) 
            return;
        AudioClip clip = obj as AudioClip;
        if (null == clip)
            return;
        sfxSource.PlayOneShot(clip);
    }

    #region Control Sound

    public void ControlSound(UtilEnums.SOUNDS _soundType, float _value)
    {
        float setValue = Mathf.Log10(_value) * 20;
        switch (_soundType)
        {
            case UtilEnums.SOUNDS.MASTER:
                masterVolume = _value;
                audioMixer.SetFloat("Master", setValue);
                break;
            case UtilEnums.SOUNDS.BGM:
                bgmVolume = _value;
                audioMixer.SetFloat("Bgm", setValue);
                break;
            case UtilEnums.SOUNDS.SFX:
                sfxVolume = _value; 
                audioMixer.SetFloat("Sfx", setValue);
                break;
        }
    }

    public float GetSoundVolume(UtilEnums.SOUNDS _soundType)
    {
        switch (_soundType)
        {
            case UtilEnums.SOUNDS.MASTER:
                return masterVolume;
            case UtilEnums.SOUNDS.BGM:
                return bgmVolume;
            case UtilEnums.SOUNDS.SFX:
                return sfxVolume;
        }
        return masterVolume;
    }

    #endregion
}
