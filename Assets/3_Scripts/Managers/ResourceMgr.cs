using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Foreach : 객체로 찾는다. 느리면 되는곳에서 쓰면 됨 (인벤토리..등, 인게임에선 쓰면 안됨)
// For는 Foreach보다 최소 10배 빠르다

public partial class ResourceMgr 
{
    public void Init()
    {
        SharedMgr.ResourceMgr = this;
    }

    /// <summary>
    /// Path : Resources/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_path"></param>
    /// <returns></returns>
    public T LoadResource<T>(string _path) where T : Object
    {
        T loadT = Resources.Load<T>(_path);
        if (loadT == null) return null;
        return loadT;
    }

    /// <summary>
    /// 팩토리 패턴
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    public BaseCharacter GetCharacter(int _id) 
    {
        // switch에서 골라서 사용
        return null; 
    }

    public T[] LoadAllResource<T>(string _path) where T: Object
    {
        T[] loadDatas = Resources.LoadAll<T>(_path);
        return loadDatas;   
    }
}
