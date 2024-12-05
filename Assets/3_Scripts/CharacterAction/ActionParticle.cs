using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionParticle : MonoBehaviour
{
    [SerializeField] E_PARTICLES particleKey;
    [SerializeField] ParticleSystem particle;

    public E_PARTICLES GetParticleKey { get { ResetParticleState(); return particleKey; } }

    public void ResetParticleState() 
    {
        if (particle.gameObject.activeSelf == true) 
            particle.gameObject.SetActive(false); 
    }

    public void DoParticle()
    {
        particle.gameObject.SetActive(true);
        particle.Play();
    }

    public void StopParticle()
    {
        particle.Stop();
        particle.gameObject.SetActive(false);
    }

    /// <summary>
    /// 파티클이 실행될 위치와 방향을 설정한 후 실행
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    public void SetParticlePosition(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        DoParticle();
    }

    //public void SetParticleState(bool _isLoop)
    //{
    //    particle.loop = _isLoop;
    //}
}
