using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SharedMgr 
{
    public static SuperMgr SuperMgr = null;
    public static SceneMgr SceneMgr = null;
    public static UIMgr UIMgr = null;
    public static ResourceMgr ResourceMgr = null;

    // 아직 안씀
    public static HoldItemMgr HoldItemMgr = null;

    //public static TableMgr InitTableMgr()
    //{
        // tablemgr==null이면 table을 만들어서 연결할때 return 한다.
        // 프로그래머가 메모리를 원할때 할당, 해제하고 싶다면 monobehaviour를 상속받으면 안된다.
    //}
}
