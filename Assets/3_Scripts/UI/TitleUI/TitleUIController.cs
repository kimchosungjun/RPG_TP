using UnityEngine;
using UtilEnums;

public class TitleUIController : MonoBehaviour
{
    [SerializeField] TitleInfoView titleInfoView;
    [SerializeField] TitleGameNameView titleGameNameView;

    private void Awake()
    {
        if(titleInfoView!=null) titleInfoView.Init(this);
        if(titleGameNameView!=null) titleGameNameView.Init(this);
    }

    private void Start()
    {
        if (titleInfoView != null) titleInfoView.FadeInfo();
    }

    public void CarryOnGameName() { titleGameNameView.FadeInfo(); }
    public void CarryOnNextGameScene() { SharedMgr.SceneMgr.LoadScene(SCENES.LOGIN, false); }    
}
