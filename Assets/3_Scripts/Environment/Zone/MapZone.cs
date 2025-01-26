using UnityEngine;
using UtilEnums;

public class MapZone : MonoBehaviour
{
    [SerializeField] ZONE_TPYES zone;

    int playerLayer = (int)LAYERS.PLAYER;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
            EnterZone();
    }

    public void EnterZone()
    {
        SharedMgr.GameCtrlMgr.GetZoneCtrl.ChangeZone(zone);
    }
}
