using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResourceMgr 
{
    public void Init()
    {
        SharedMgr.ResourceMgr = this;
    }

    public T LoadResource<T>(string _path) where T : Object
    {
        T loadT = Resources.Load<T>(_path);
        if (loadT == null) return null;
        return loadT;
    }
}
