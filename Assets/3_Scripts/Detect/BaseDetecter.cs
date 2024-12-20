using UnityEngine;

public class BaseDetecter : MonoBehaviour
{
    [SerializeField] protected LayerMask bit_DetectLayer;
    [SerializeField] protected int int_DetectLayer;
    [SerializeField] protected float detectRange;
    [SerializeField] protected bool isDetect =false;
    [SerializeField] Transform targetTransform = null;
    public Transform GetTransform { get { return targetTransform; } }

    public void Setup(float _detectRange, int _int_DetectLayer)
    {
        detectRange = _detectRange;
        bit_DetectLayer = 1<<_int_DetectLayer;
        int_DetectLayer = _int_DetectLayer;
    }

    public void ChangeRange(float _detectRange)
    {
        detectRange = _detectRange;
    }

    public bool IsDetect()
    {
        return isDetect;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if(other.gameObject.layer == int_DetectLayer)
        {
            if(targetTransform == null)
                targetTransform = other.transform;  
            isDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == int_DetectLayer)
        {
            //targetTransform = null;
            isDetect = false;
        }
    }

    public float GetDistance()
    {
        return Vector3.Distance(transform.position, targetTransform.position);
    }
}
