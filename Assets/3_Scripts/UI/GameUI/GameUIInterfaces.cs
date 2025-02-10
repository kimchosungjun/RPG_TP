using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputKeyUI
{
    public void InputKey();
}

public interface ITurnOnOffUI
{
    public void TurnOn();
    public void TurnOff();
}

public interface IUpdateUI
{
    public void UpdateData();
}