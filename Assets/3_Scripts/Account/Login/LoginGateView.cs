using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginGateView : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] GameObject gateParent;
    [SerializeField, Tooltip("0:Left, 1:Right, 2:Alpha")] Image[] gateImages;
    public void Init()
    {
        if(anim == null) anim =GetComponent<Animator>();
        gateImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Gate_Atlas", "LeftGate");
        gateImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Gate_Atlas", "RightGate");
        gateImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Gate_Atlas", "AlphaGate");
        if (gateParent.activeSelf == false) gateParent.SetActive(true);
    }

    public void OpenGate()
    {
        anim.Play("Gate_Open");
    }

    public void InActiveGate()
    {
        gateParent.SetActive(false);
    }

    internal void CloseDoor()
    {
        if (gateParent.activeSelf == false)
            gateParent.SetActive(true);

        anim.Play("Gate_Close");
    }

    public void ActiveLogin()
    {
        SharedMgr.UIMgr.LoginUICtrl.GetLoginInputView.ActiveView();
    }
}
