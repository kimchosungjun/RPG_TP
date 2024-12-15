using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [SerializeField] PlayerStatusUI playerStatusUICtrl;
    public PlayerStatusUI GetPlayerStatusUICtrl { get { return playerStatusUICtrl; } }

    private void Start()
    {
        UIInit();
    }
    
    public void UIInit()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        if(playerStatusUICtrl==null) playerStatusUICtrl = GetComponentInChildren<PlayerStatusUI>();
    }
}
