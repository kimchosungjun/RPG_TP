using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAction : MonoBehaviour
{
    [SerializeField] Transform particleParent;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] float maintainTime;

    public HitBox GetHitBox { get; set; } = null;
    public void DoParticle(float _maintainTime, bool _followPlayer = false)
    {
        if (particleParent.gameObject.activeSelf == false)
            particleParent.gameObject.SetActive(true);
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Play();
        }
        if (_followPlayer)
        {
            StartCoroutine(CFollowPlayer(_maintainTime));
        }
        else
        {
            StartCoroutine(CParticleMaintainTime(_maintainTime));
        }
    }

    

    public void DoParticle()
    {
        if (particleParent.gameObject.activeSelf == false)
            particleParent.gameObject.SetActive(true);
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Play();
        }
        StartCoroutine(CParticleMaintainTime(maintainTime));
    }

    IEnumerator CParticleMaintainTime(float _maintainTime) 
    {
        yield return new WaitForSeconds(_maintainTime);
        StopParticle();
    }

    IEnumerator CFollowPlayer(float _mainTainTime)
    {
        float time = 0;
        while (time < _mainTainTime)
        {
            particleParent.transform.position = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position + Vector3.up *0.2f;
            time += Time.deltaTime;
            yield return null;
        }
        StopParticle(); 
    }

    public void DoParticleLoop()
    {
        if(particleParent.gameObject.activeSelf==false)
            particleParent.gameObject.SetActive(true);
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Play();
        }
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

    public void SetParticlePosition(Vector3 _position, Quaternion _rotation)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        DoParticle(maintainTime);
    }

    public void SetParticleTime() { StartCoroutine(CSetParticleTime()); }

    IEnumerator CSetParticleTime() 
    {
        yield return new WaitForSeconds(maintainTime);
        int cnt = particles.Length;
        for (int i = 0; i < cnt; i++)
        {
            particles[i].Stop();
        }
        particleParent.gameObject.SetActive(false);
    }

}
