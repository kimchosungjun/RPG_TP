using System;

public partial class Enums
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

    public static T ConvertStringToEnum<T>(string _enumString, out T result) where T : struct, Enum
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


// 오브젝트 풀링
namespace PoolEnums
{
    public enum  OBJECTS
    {
        WARRIOR_NORMAL=0,
        ATTACK_BUFF=1,
        WARRIOR_SLASH = 2,
        ARCHER_NORMAL=3,
        ARCHER_ATTACK_SKILL=4,
        ARCHER_POP=5,
        ARCHER_ULTIMATE = 6,
        MAGICIAN_NORMAL =7
    }
}

// 레이어, 씬, 테이블 ID
namespace UtilEnums
{
    public enum SCENES
    {
        TITLE = 0,
        LOGIN = 1,
        LOADING = 2,
        GAME = 3,
    }

    public enum LAYERS
    {
        DEFAULT = 0,
        WALL = 3,
        WATER = 4,
        UI = 5,
        GROUND = 6,
        MONSTER = 7,
        PLAYER = 8,
        NPC = 9,
        INTERACTOBJECT = 10,
    }

    public enum TABLE_FOLDER_TYPES
    {
        NONE = 0,
        PLAYER = 1,
        MONSTER = 2,
        ITEM = 3,
        NPC = 4
    }
}

// UI
namespace UIEnums
{
    public enum STATUS
    {
        HP,
        EXP,
        LEVEL
    }
}