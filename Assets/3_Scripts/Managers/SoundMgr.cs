using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    AudioSource bgmSource;
    AudioSource effectSource;

    public void Init()
    {
        if(bgmSource==null)bgmSource=transform.Find("BGM").GetComponent<AudioSource>();
        bgmSource.loop = true;
        if(effectSource == null) effectSource = transform.Find("Effect").GetComponent<AudioSource>();
        SharedMgr.SoundMgr = this;

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
        effectSource.PlayOneShot(clip);
    }
}
