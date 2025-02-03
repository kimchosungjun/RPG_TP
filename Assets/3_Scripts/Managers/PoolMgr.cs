using System.Collections.Generic;
using UnityEngine;
using PoolEnums;

public class PoolMgr : MonoBehaviour
{
    /*********** Vale ***********/

    #region Object Manage Data Struct
    // Pool Parent Group
    Dictionary<OBJECTS, Transform> poolParentGroup = new Dictionary<OBJECTS, Transform>();
    // Object Pool Groupww
    Dictionary<OBJECTS, List<Transform>> objectPoolGroup = new Dictionary<OBJECTS, List<Transform>>();    
    // Pool Original Group
    Dictionary<OBJECTS, Transform> originalGroup = new Dictionary<OBJECTS, Transform>();
    #endregion

    #region UI Pool
    [Header("Float Damage Text")]
    [SerializeField] Transform floatDamageParent;
    [SerializeField] FloatDamageTextUI floatDamageOriginal;
    [SerializeField] List <FloatDamageTextUI> floatDamageTexts = new List<FloatDamageTextUI>();
    ShowGetItemSlot[] showGetItemSlots = null;
    #endregion

    /********* Methods **********/

    #region Life Cycle
    private void Awake()
    {
        SharedMgr.PoolMgr = this;
    }

    private void Start()
    {
        ShowGetItemUI showGetItemUI = SharedMgr.UIMgr.GameUICtrl.GetShowGetItemUI;
        if(showGetItemUI==null) showGetItemUI = FindObjectOfType<ShowGetItemUI>();  
        showGetItemSlots = showGetItemUI.GetSlots();
    }
    #endregion

    #region Get Pool

    /// <summary>
    /// Return Active State
    /// </summary>
    /// <param name="_poolObject"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    /// <returns></returns>
    public Transform GetPool(OBJECTS _poolObject, Vector3 _position, Quaternion _rotation)
    {
        Transform result = GetPool(_poolObject);
        result.position = _position;
        result.rotation = _rotation;
        result.gameObject.SetActive(true);
        return result;
    }

    /// <summary>
    /// Return Active State
    /// </summary>
    /// <param name="_poolObject"></param>
    /// <param name="_transform"></param>
    /// <returns></returns>
    public Transform GetPool(OBJECTS _poolObject, Transform _transform)
    {
        Transform result = GetPool(_poolObject);
        result.position = _transform.position;
        result.rotation = _transform.rotation;
        result.gameObject.SetActive(true);
        return result;
    }

    /// <summary>
    /// Return InActive State
    /// </summary>
    /// <param name="_poolObject"></param>
    /// <returns></returns>
    public Transform GetPool(OBJECTS _poolObject)
    {
        Transform result = null;
        if (poolParentGroup.ContainsKey(_poolObject) == false)
        {
            string parentName = Enums.GetEnumString<OBJECTS>(_poolObject) + "Parent";

            GameObject go = new GameObject(parentName);
            go.transform.position = Vector3.zero;
            go.transform.SetParent(this.transform, true); // Second Parameter = true : maintain own world position
            poolParentGroup.Add(_poolObject, go.transform);

            result = SharedMgr.ResourceMgr.LoadResource<Transform>("Pools/" + Enums.GetEnumString<OBJECTS>(_poolObject));
            originalGroup.Add(_poolObject, result);
            result = Instantiate(result.gameObject).transform;
            result.SetParent(go.transform, true);

            objectPoolGroup[_poolObject] = new List<Transform>();
            objectPoolGroup[_poolObject].Add(result);
        }
        else
        {
            result = GetObjectInPool(_poolObject);
        }
        return result;
    }

    Transform GetObjectInPool(OBJECTS _poolObject)
    {
        List<Transform> list = objectPoolGroup[_poolObject];   
        int cnt = list.Count;
        for(int i = 0; i < cnt; i++)
        {
            if (list[i].gameObject.activeSelf == false)
                return list[i];
        }

        GameObject result = Instantiate(originalGroup[_poolObject].gameObject);
        list.Add(result.transform);
        result.transform.SetParent(poolParentGroup[_poolObject], true);
        return result.transform;
    }
    #endregion

    #region UI Pool

    public FloatDamageTextUI GetFloatDamageText()
    {
        int cnt = floatDamageTexts.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (floatDamageTexts[i].gameObject.activeSelf == false)
            {
                floatDamageTexts[i].gameObject.SetActive(true);
                return floatDamageTexts[i];
            }
        }

        FloatDamageTextUI instText = Instantiate(floatDamageOriginal);
        instText.transform.SetParent(floatDamageParent, true);
        floatDamageTexts.Add(instText);
        return instText;
    }

    public ShowGetItemSlot GetItemSlot()
    {
        for (int i = 0; i < 2; i++)
        {
            if (showGetItemSlots[i].gameObject.activeSelf == false)
            {
                showGetItemSlots[i].transform.SetAsLastSibling();
                return showGetItemSlots[i];
            }
        }
        return null;
    }



    #endregion
}
