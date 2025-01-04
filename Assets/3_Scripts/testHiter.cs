using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testHiter : MonoBehaviour
{
    [SerializeField] TransferAttackData attackData;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)UtilEnums.LAYERS.PLAYER)
        {
            other.GetComponent<BasePlayer>().TakeDamage(attackData);
        }
    }
}
