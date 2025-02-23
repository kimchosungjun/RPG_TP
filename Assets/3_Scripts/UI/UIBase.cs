using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;

public class UIBase : MonoBehaviour
{
    List<UIBase> uiBaseSet;
    
    public virtual void InputKey() { }
    public virtual void TurnOn() { }
    public virtual void TurnOff() { }
    public virtual void UpdateData() { }
    public virtual void OpenAnimation() { }
    public virtual void CloseAnimation() { }

    public UIBase() { uiBaseSet = new List<UIBase>(); }

    public void AddUI(UIBase _uiBase)
    {
        if (_uiBase == null) return;
        uiBaseSet.Add(_uiBase);
    }

    public UIBase GetUIBase(GAMEUI _uiType)
    {
        int type = (int)_uiType;
        if (uiBaseSet.Count <= type)
            return null;
        return uiBaseSet[type];
    }
}
