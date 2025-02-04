using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;

public partial class SoundMgr : MonoBehaviour
{
    Dictionary<BGMCLIPS,AudioClip> bgmClipGroup = new Dictionary<BGMCLIPS, AudioClip> ();
    Dictionary<SFXCLIPS, AudioClip> sfxClipGroup = new Dictionary<SFXCLIPS, AudioClip>();

    #region Check Have Clip Data
    public bool CheckClip(BGMCLIPS _bgmClips)
    {
        if (bgmClipGroup.ContainsKey(_bgmClips) == false)
            return false;
        return true;
    }

    public bool CheckClip(SFXCLIPS _sfxClips)
    {
        if (sfxClipGroup.ContainsKey(_sfxClips) == false)
            return false;
        return true;
    }
    #endregion

    #region Load Clip 
    private AudioClip LoadClip(BGMCLIPS _bgmClip) 
    {
        string path = "BGM/" + Enums.GetEnumString<BGMCLIPS>(_bgmClip);
        AudioClip clip = SharedMgr.ResourceMgr.LoadResource<AudioClip>(path);
        bgmClipGroup.Add(_bgmClip, clip);
        return clip;
    }

    private AudioClip LoadClip(SFXCLIPS _sfxClip)
    {
        string path = "BGM/" + Enums.GetEnumString<SFXCLIPS>(_sfxClip);
        AudioClip clip = SharedMgr.ResourceMgr.LoadResource<AudioClip>(path);
        sfxClipGroup.Add(_sfxClip, clip);
        return clip;
    }
    #endregion

    #region Get Clip
    public AudioClip GetClip(BGMCLIPS _bgmClip)
    {
        if (CheckClip(_bgmClip))
            return bgmClipGroup[_bgmClip];
        else
            return LoadClip(_bgmClip);  
    }

    public AudioClip GetClip(SFXCLIPS _sfxClip)
    {
        if (CheckClip(_sfxClip))
            return sfxClipGroup[_sfxClip];
        else
            return LoadClip(_sfxClip);
    }
    #endregion

    public void ClearClips()
    {
        if (bgmClipGroup.Count != 0) 
            bgmClipGroup.Clear();
        if(sfxClipGroup.Count!=0)
            sfxClipGroup.Clear();   
    }
}
