using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SubBossStatusUI : EliteMonsterStatusUI
{
    Transform camTransform;

    public override void CamTransform()
    {
        camTransform = Camera.main.transform;
    }
}
