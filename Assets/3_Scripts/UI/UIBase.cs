using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIEnums;

public class UIBase : MonoBehaviour
{
    protected bool isActiveState = false;
    public bool GetIsActiveState { get { return isActiveState; } }

    public virtual void InputKey()  { }
    public virtual void TurnOn() { }
    public virtual void TurnOff() { }
    public virtual void UpdateData() { }
    public virtual void OpenAnimation() { }
    public virtual void CloseAnimation() { }
}