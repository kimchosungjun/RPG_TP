using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public partial class SoundMgr : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSource[] bgmSources;
    AudioSource sfxSource;

    float masterVolume = 0f;
    float bgmVolume = 0f;
    float sfxVolume = 0;

    public void Init()
    {
        bgmSources[0].loop = true;
        bgmSources[1].loop = true;
        if(sfxSource == null) sfxSource = transform.Find("SFX").GetComponent<AudioSource>();
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
        if (bgmSources[0].clip !=null)
        {
            bgmSources[0].Stop();
            bgmSources[0].clip = null;
        }    

        _bgm = "BGM/" + _bgm;
        Object obj = Resources.Load(_bgm);
        if (obj == null)    // obj = null 이 성립되기 때문에 실수로 지우면 사운드 플레이 버그가 발생하지만 찾기가 어렵다.
            return;
        AudioClip clip = obj as AudioClip;
        if (null == clip)
            return;
        bgmSources[0].clip = clip;
        bgmSources[0].Play();
    }

    public void PlayBGM(UtilEnums.BGMCLIPS _bgmClip)
    {
        
    }

    IEnumerator CFadeBGM()
    {
        float time = 0f;

        yield return new WaitForFixedUpdate();
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
