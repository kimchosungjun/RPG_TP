using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseControl  
{
    List<UIBase> uiBaseSet;
    Stack<UIBase> uiPopupSet;

    public UIBaseControl()
    {
        uiBaseSet = new List<UIBase>();
        uiPopupSet = new Stack<UIBase>();
    }

    #region Manage UI Base 
    public void AddUIBase(UIBase _uiBase)
    {
        if (_uiBase == null) return;
        uiBaseSet.Add(_uiBase);
    }

    public UIBase GetUIBase(GameUICtrl.InputKeyUITypes _uiType)
    {
        int type = (int)_uiType;
        if (uiBaseSet.Count <= type)
            return null;
        return uiBaseSet[type];
    }
    #endregion

    #region Manage UI Popup
    public void PushUIPopup(UIBase _uiBase)
    {
        if (_uiBase == null) return;
        uiPopupSet.Push(_uiBase);
        _uiBase.transform.SetAsLastSibling();
    }

    public UIBase PopUIPopup()
    {
        if (uiPopupSet.Count == 0)
            return null;
        return uiPopupSet.Pop();
    }

    public UIBase PeekUIPopup()
    {
        if (uiPopupSet.Count == 0)
            return null;
        return uiPopupSet.Peek();
    }

    public bool IsOpenUI()
    {
        if (uiPopupSet.Count == 0) return false;
        return true;
    }

    public bool IsHaveActiveUI()
    {
        int cnt = uiBaseSet.Count;
        for(int i=0; i<cnt; i++)
        {
            if (uiBaseSet[i].GetIsActiveState)
                return true;
        }
        return false;
    }
    #endregion
}
