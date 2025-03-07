using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ResourceMgr
{
    #region Monster Factory
    public BaseMonster MonsterFactory(string _path)
    {
        BaseMonster monster = LoadResource<BaseMonster>(_path);
        if (monster == null)
            return null;
        GameObject go = Object.Instantiate(monster).gameObject;
        monster = go.GetComponent<BaseMonster>();
        return monster;
    }

    public BaseMonster MonsterFactory(string _path, Vector3 _position, Quaternion _rotation)
    {
        BaseMonster monster = LoadResource<BaseMonster>(_path);
        if (monster == null)
            return null;
        GameObject go = Object.Instantiate(monster).gameObject;
        monster = go.GetComponent<BaseMonster>();
        go.transform.position = _position;
        go.transform.rotation = _rotation;  
        return monster;
    }

    public BaseMonster MonsterFactory(string _path, Transform _transform)
    {
        BaseMonster monster = LoadResource<BaseMonster>(_path);
        if (monster == null)
            return null;
        GameObject go = Object.Instantiate(monster).gameObject;
        monster = go.GetComponent<BaseMonster>();
        go.transform.position = _transform.position;
        go.transform.rotation = _transform.rotation;
        return monster;
    }
    #endregion
}
