using UnityEngine;
using UtilEnums;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    [SerializeField] AudioSource source = null;
    private void Awake()
    {
        if(source==null)
            source = GetComponent<AudioSource>();   
    }

    private void Start()
    {
        source.spatialBlend = 1;
        source.playOnAwake = false;
        source.loop = false;
    }

    public void PlayOneSFX(SFXCLIPS _sfxClip) 
    {
        AudioClip clip = SharedMgr.SoundMgr.GetClip(_sfxClip);
        source.PlayOneShot(clip); 
    }
}
