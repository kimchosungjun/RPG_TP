using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinder : Finder
{
    public Transform Target { get; set; }

    protected override void Awake()
    {
        setFindLayer = UtilEnums.LAYERS.INTERACTOR;
        base.Awake();
    }

    public float GetDistance()
    {
        if (Target == null) return -1;

        return Vector3.Distance(transform.position,
            Target.position);
    }

    public override void DetectInSight()
    {
        if (GetDistance() > sightRange) { isInSight = false; return; }

        Collider[] colls = Physics.OverlapSphere(transform.position, sightRange, findLayerMask);
        int cnt = colls.Length;
        if (cnt == 0) isInSight = false;
        else
        {
            Vector3 direction = colls[0].transform.position - transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(transform.forward, direction);

            if (Mathf.Abs(angle) <= sightAngle)
                isInSight = true;
            else
                isInSight = false;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.AddInteractable(other.GetComponent<Interactable>());
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == (int)UtilEnums.LAYERS.INTERACTOR)
        {
            SharedMgr.InteractionMgr.RemoveInteractable(other.GetComponent<Interactable>());
        }
    }
}
