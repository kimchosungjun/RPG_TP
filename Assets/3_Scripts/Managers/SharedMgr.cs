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

    #region Stick Game Scene
    public static PoolMgr PoolMgr = null;
    public static GameCtrlMgr GameCtrlMgr = null;
    #endregion
}
