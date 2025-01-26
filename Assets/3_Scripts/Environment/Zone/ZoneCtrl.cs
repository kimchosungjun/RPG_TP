using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilEnums;

public class ZoneCtrl : MonoBehaviour
{
    [SerializeField] ZONE_TPYES cuurentZone;

    public void ChangeZone(ZONE_TPYES _currentZone)
    {
        if (_currentZone == cuurentZone) return;
        
    }
}
