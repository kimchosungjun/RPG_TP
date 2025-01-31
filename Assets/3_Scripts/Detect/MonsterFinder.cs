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
        if(GetDistance()>sightRange) { isInSight = false; return; }

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

    public bool DetectInSihgt(float _range)
    {
        {
            if (GetDistance() > _range)
                return false;

            Collider[] colls = Physics.OverlapSphere(transform.position, _range, findLayerMask);
            int cnt = colls.Length;
            if (cnt == 0)
                return false;
            else
            {
                Vector3 direction = colls[0].transform.position - transform.position;
                direction.y = 0;
                float angle = Vector3.Angle(transform.forward, direction);

                if (Mathf.Abs(angle) <= sightAngle)
                    return true;
                else
                    return false;
            }
        }
    }
    #endregion

    #region Detect

    public bool IsDetect() { return (GetDistance() < sightRange) ? true : false; }
    #endregion
}
