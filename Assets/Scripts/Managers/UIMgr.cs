using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr 
{
    public LoadingUICtrl LoadingUICtrl { get; set; } = null;
    public LoginUICtrl LoginUICtrl { get; set; } = null;
    
    public void Init() { SharedMgr.UIMgr = this; }
}
