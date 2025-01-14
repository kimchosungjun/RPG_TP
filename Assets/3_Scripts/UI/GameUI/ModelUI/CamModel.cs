using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamModel : MonoBehaviour
{
    [SerializeField] Animator anim;

    public void DoIdle()
    {
        anim.SetBool("Active", false);
    }

    public void DoAction()
    {
        anim.SetBool("Active", true);
    }

    public void Active()
    {
        if (gameObject.activeSelf ) return;
        this.gameObject.SetActive(true);   
    }

    public void InActive()
    {
        if (gameObject.activeSelf == false) return;
        this .gameObject.SetActive(false);   
    }
}
