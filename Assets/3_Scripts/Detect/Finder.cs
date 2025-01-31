using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Finder : MonoBehaviour
{
    [SerializeField] protected float sightRange;
    [SerializeField] protected float sightAngle;
    
    protected int findLayer;
    protected LayerMask findLayerMask;

    protected bool isInSight = false;
    protected bool isDetect = false;

    public void ChangeDetectLayer(UtilEnums.LAYERS _layer)
    {
        if(_layer == UtilEnums.LAYERS.DEFAULT)
        {
            findLayer = 0;
            findLayerMask = 0;
        }
        else
        {
            int layer = (int)_layer;
            findLayer = layer;
            findLayerMask = 1 << layer;
        }
    }

    public void ChangeRange(float _range) { sightRange = _range; }   
    public void DecreaseRange(float _percent) { sightRange *= _percent; }

    public virtual void SetDetect(float _sightRange, float _sightAngle, UtilEnums.LAYERS _layer)
    {
        sightRange = _sightRange;
        sightAngle = _sightAngle;
        ChangeDetectLayer(_layer);
    }

    public abstract void DetectInSight();
}
