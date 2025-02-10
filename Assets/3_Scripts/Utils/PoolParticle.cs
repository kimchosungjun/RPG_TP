using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolParticle : MonoBehaviour
{
    [Header("Particle Objects")]
    [SerializeField, Tooltip("0:LevleUp")] ParticleAction[] particles;
   
    public void UseParticle(PoolEnums.ONLYONE _type)
    {
        particles[(int)_type].DoParticle();
    }

    public void UseParticle(PoolEnums.ONLYONE _type, float _maintainTime, bool _isFollowPlayer)
    {
        particles[(int)_type].DoParticle(_maintainTime, _isFollowPlayer);
    }
}
