using UnityEngine;

public class MonsterFinder : Finder
{
    // Common
    public float GetDistance()
    {
        return Vector3.Distance(transform.position, 
            SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform.position);
    }

    #region Sight

    public bool IsInSight() { return isInSight; }
    public override void DetectInSight()
    {
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
    #endregion

    #region Detect
 
    public bool IsDetect() { return isDetect; }

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == findLayer)
            isDetect = true;
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == findLayer)
            isDetect = false;
    }
    #endregion

}
