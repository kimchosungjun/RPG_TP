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
        // PLAYER
        WARRIOR_CAST = 0,
        WARRIOR_SLASH,
        ARCHER_NORMAL,
        ARCHER_SLASH,
        MAGE_CAST,
        MAGE_METEOR,
        MAGE_CIRCLE_SPELL,
        PITCHBLACK_POP,
        GRASS_POP,
        FIRE_POP,
        ATTACK_BUFF,
        HEAL_BUFF,
        
        // MONSTER
        MON_BITE,
        MON_RUSH,
        VIRUS_SPREAD,
        GUIDED_FIRE,
        
        // COMMON
        EXPLOSION,
        STUN,
        HEAL,
        NONE
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

    public enum SOUNDS
    {
        MASTER=0,
        BGM=1,
        SFX=2
    }

    public enum BGMCLIPS
    {
        LOGIN_BGM,
        CAVE_BGM,
        TOWN_BGM,
        DRAGONNEST_BGM,
        NONE
    }

    public enum SFXCLIPS
    {
        ARCHER_ATK_SFX = 0,
        BUFF_SFX,
        BUTTON_SFX,
        DRAGON_FIRE_SFX,
        DRAGON_GROWL_SFX,
        DRAGON_GUIDED_SFX,
        EXPLOSION_SFX,
        FAIL_SFX,
        GOLD_SFX,
        HEAL_SFX,
        HIT_SFX,
        INVEN_OPEN_SFX,
        INVEN_CLOSE_SFX ,
        MAGE_FIRE_SFX,
        MON_ATK_SFX,
        MON_SLASH_SFX,
        PLAYER_ATK_SFX,
        PLAYERALL_DEATH_SFX,
        VIRUS_SPREAD_SFX
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
        INTERACTOR = 9,
        UNINTERACTOR = 10,
    }

    public enum TABLE_FOLDER_TYPES
    {
        NONE = 0,
        PLAYER = 1,
        MONSTER = 2,
        ITEM = 3,
        NPC = 4,
        CAMERA=5,
    }

    public enum ZONE_TPYES
    {
        CAVE=0,
        TOWN_SOUTH=1,
        TOWN_EAST=2,
        TOWN_WEST=3
    }

    public enum PATH_TYPES
    {
        LEFT_TOWN=0,
        RIGHT_TOWN=1
    }
}

// UI
namespace UIEnums
{
    public enum STATUS
    {
        HP,
        EXP,
        LEVEL,
        GROGGY
    }

    public enum CHANGE
    {
        COOLDOWN,
        DEATH,
        CANNOTCHANGE,
    }

    public enum PARTY
    {
        STATUS =0,
        WEAPON =1,
        SKILL_UPGRADE =2,
        MAX=3
    }

    public enum GAMEUI
    {
        PLAYER_STATUS,
        PLAYER_CHANGE,
        INVENTORY,
        GETITEM,
        INTERACT,
        PLAYER_PARTY,
        UPGRADE,
        QUEST,
        DIALOGUE,
        SOUND,
        NONE
    }
}