using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStepPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] footStepClips;
    private int lastClip = -1;

    public void SoundFootStep()
    {
        audioSource.Stop();
        int curClip;
        do
        {
            curClip = Random.Range(0, footStepClips.Length);
        } while (lastClip == curClip);

        audioSource.clip = footStepClips[curClip];
        audioSource.Play();
        lastClip = curClip;
    }
}
