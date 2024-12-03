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

public enum E_SCENES
{
    TITLE = 0,
    LOGIN =1,
    LOADING = 2,
    GAME=3,
}

public enum E_LAYERS
{
    DEFAULT=0,
    WALL=3,
    WATER=4,
    UI=5,
    GROUND=6,
    MONSTER=7,
    PLAYER=8,
    NPC=9,
    INTERACTOBJECT=10,
}

public enum E_PLAYER_STATES
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

public enum E_BTS
{
    SUCCESS=0,
    FAIL=1,
    RUNNING=2
}

public enum E_MONSTER_ANIMS
{
    IDLE=0,
    MOVE=1,
    HIT=2,
    ATTACK=3,
    DEATH=4,
    GROGGY=5
}

public enum E_ATTACKTYPE
{
    NORMAL=0,
    KNOCKBACK=1,
    STUN=2,
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


