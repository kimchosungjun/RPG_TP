using System;

public class Enums
{
    public static string GetEnumString<T>(T _enumType) where T : Enum
    {
        return Enum.GetName(typeof(T), _enumType); ;
    }
    public static int GetIntValue<T>(T _enumValue) where T : Enum
    {
        return Convert.ToInt32(_enumValue);
    }
    public static int GetEnumLenth<T>() where T : Enum
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }
}


public enum E_SCENE
{
    SCENE_TITLE=0,
    SCENE_GAME=1,
    SCENE_UI=2,
    SCENE_LOGIN,
}

public enum E_CAMERAVIEW
{
    CAMERA_QUATERVIEW=0,
    CAMERA_CLOSEUP=1,
}