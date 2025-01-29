using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMgr 
{
    public void Init()
    {
        Cursor.visible = false;
        SharedMgr.CursorMgr = this;
    }

    public void SetCursor()
    {
        Texture2D cursorTexture = SharedMgr.ResourceMgr.LoadResource<Texture2D>("Textures/Cursor_Hand_64");
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetCursorVisibleState(bool _isVisible)
    {
        Cursor.visible = _isVisible;
    }
}
