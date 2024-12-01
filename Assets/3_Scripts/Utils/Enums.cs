using System;

public class Enums
{
    public static string GetEnumString<T>(T _enumType) where T : Enum
    {
        return Enum.GetName(typeof(T), _enumType); 
    }
    public static int GetIntValue<T>(T _enumValue) where T : Enum
    {
        return Convert.ToInt32(_enumValue);
    }
    public static int GetEnumLenth<T>() where T : Enum
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }

    public static T TryGetEnumValue<T>(string _enumString, out T result) where T : struct, Enum
    {
        try
        {
            result = (T)Enum.Parse(typeof(T), _enumString, ignoreCase: true);
            return result;
        }
        catch
        {
            result = default;
            return default(T);
        }
    }
}

public enum E_SCENE
{
    SCENE_TITLE = 0,
    SCENE_LOGIN =1,
    SCENE_LOADING = 2,
    SCENE_GAME=3,
}

public enum E_PLAYER_FSM
{
    MOVEMENT=0,
    DASH=1,
    JUMP=2,
    FALL=3,
    ATTACK=4,
    SKILL=5,
    ULTIMATESKILL=6,
    HIT=7,
    DEATH=8,
    INTERACTION=9,
    MAX=10
}

public enum E_BT
{
    BT_SUCCESS=0,
    BT_FAIL=1,
    BT_RUNNING=2
}

public enum E_MONSTER_ANIM
{
    IDLE=0,
    MOVE=1,
    HIT=2,
    DEATH=3,
    ATTACK=4,
}

#region ITEM

public enum E_CAMERAVIEW
{
    CAMERA_QUATERVIEW=0,
    CAMERA_CLOSEUP=1,
}

public enum E_ITEMTYPE
{
    ITEM_NONE=0,
    ITEM_WEAPON = 1,
    ITEM_COMSUMPTION = 2,
    ITEM_OTHERS = 3
}

public enum E_WEAPONTYPE
{
    WEAPON_NONE = 0,
    WEAPON_SWORD = 1,
    WEAPON_BOW = 2,
    WEAPON_STAFF = 3,
}

public enum E_WEAPONEFFECT
{
    WEAPON_NONE = 0,
    WEAPON_CRITICAL = 1, 
    WEAPON_ABSORPTION = 2
}

#endregion

public enum E_CHARACTER
{
    PLAYER=0,
    NORMAL_MONSTER=1,
    BOSS_MONSTER=2,
    NPC=3
}

