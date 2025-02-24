using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SharedMgr 
{
    public static MainCamera mainCam;

    #region Stick Super Mgr  
    public static MgrCreator SuperMgr = null;
    public static SceneMgr SceneMgr = null;
    public static UIMgr UIMgr = null;
    public static ResourceMgr ResourceMgr = null;
    public static TableMgr TableMgr = null;
    public static SoundMgr SoundMgr = null;
    public static InventoryMgr InventoryMgr = null; 
    public static PhotonMgr PhotonMgr = null;
    public static InteractionMgr InteractionMgr = null;
    public static QuestMgr QuestMgr = null;
    public static SaveMgr SaveMgr = null;
    public static CursorMgr CursorMgr = null;
    #endregion

    //public static TableMgr InitTableMgr()
    //{
    // tablemgr==null이면 table을 만들어서 연결할때 return 한다.
    // 프로그래머가 메모리를 원할때 할당, 해제하고 싶다면 monobehaviour를 상속받으면 안된다.
    //}

    #region Stick Game Scene
    public static PoolMgr PoolMgr = null;
    public static GameCtrlMgr GameCtrlMgr = null;
    #endregion
}
