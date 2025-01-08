using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginGateView : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject gateParent;
    public void Init()
    {
        if(anim == null) anim =GetComponent<Animator>();
        if(gateParent.activeSelf==false) gateParent.SetActive(true);   
    }

    public void OpenGate()
    {
        anim.Play("Gate_Open");
    }

    public void InActiveGate()
    {
        gateParent.SetActive(false);
    }
}
