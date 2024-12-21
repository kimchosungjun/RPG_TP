using UnityEngine;
using UnityEngine.UI;


public class GameUICtrl : MonoBehaviour
{
    [SerializeField] PlayerStatusUI playerStatusUI;
    [SerializeField] PlayerChangeUI playerChangeUI;
    public PlayerStatusUI GetPlayerStatusUI { get { return playerStatusUI; } }
    public PlayerChangeUI GetPlayerChangeUI { get {return playerChangeUI; } }
    private void Start()
    {
        UIInit();
    }
    
    public void UIInit()
    {
        SharedMgr.UIMgr.GameUICtrl = this;
        if(playerStatusUI == null) playerStatusUI = GetComponentInChildren<PlayerStatusUI>();
    }
}
