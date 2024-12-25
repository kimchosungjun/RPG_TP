using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAction : MonoBehaviour
{
    [SerializeField] Transform particleParent;
    [SerializeField] ParticleSystem[] particles;

    public void DoParticle(float _maintainTime)
    {
        particleParent.gameObject.SetActive(true);
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Play();
        }
        StartCoroutine(CParticleMaintainTime(_maintainTime));
    }

    IEnumerator CParticleMaintainTime(float _maintainTime) 
    {
        yield return new WaitForSeconds(_maintainTime);
        StopParticle();
    }

    public void StopParticle()
    {
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Stop();
        }
        particleParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// 파티클이 실행될 위치와 방향을 설정한 후 실행
    /// </summary>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    public void SetParticlePosition(Vector3 _position, Quaternion _rotation, float _maintainTime = 1f)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        DoParticle(_maintainTime);
    }
}
