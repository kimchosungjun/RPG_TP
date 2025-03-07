using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public T[] LoadAllResource<T>(string _path) where T: Object
    {
        T[] loadDatas = Resources.LoadAll<T>(_path);
        return loadDatas;   
    }
}
