using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnoyBear : MonoBehaviour
{
    [SerializeField] SubBossStatusUICtrl ctrl;
    

    protected void Start()
    {
        ctrl.Setup(this.transform);
    }

    private void Update()
    {
        ctrl.Execute(); 
    }
}
