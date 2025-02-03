public partial class Enums { }

// 플레이어
namespace PlayerEnums
{
    public enum TYPEIDS
    {
        WARRIOR = 0,
        ARCHER = 1,
        MAGE = 2,
        NONE
    }

    public enum CONDITION_SKILLS
    {
        WARRIOR_ROAR = 0,
        MAGE_VITALITY_INCREASE = 1
    }

    public enum ATTACK_SKILLS
    {
        WARRIOR_ULTIMATE = 0,
        ARCHER_FULL_BLOWN_SHOOT = 1,
        ARCHER_ULTIMATE = 2,
        MAGE_ULTIMATE = 3
    }

    public enum STATES
    {
        MOVEMENT = 0,
        DASH = 1,
        JUMP = 2,
        FALL = 3,
        ATTACK = 4,
        SKILL = 5,
        ULTIMATESKILL = 6,
        HIT = 7,
        DEATH = 8,
        INTERACTION = 9,
        MAX = 10
    }

    public enum ENHANCES
    {
        NORMALATTACK,
        SKILL,
        ULTIMATESKILL,
        LEVEL
    }

    public enum BATTLE_TYPE
    {
        NEAR=0,
        FAR=1
    }

    public enum ACTION_TYPE
    {
        NORMAL=0,
        SKILL=1,
        ULTIMATE=2
    }
}

// 몬스터
namespace MonsterEnums 
{
    public enum STATES
    {
        IDLE = 0,
        MOVE = 1,
        HIT = 2,
        ATTACK = 3,
        DEATH = 4,
        GROGGY = 5
    }

    public enum NODESTATES
    {
        SUCCESS = 0,
        FAIL = 1,
        RUNNING = 2
    }

    public enum TYPEIDS
    {
        CHEST=0,
        VIRUS=1,
        SLIME=2,
        HORNSLIME=3,
        DRAGON=4,
    }

    public enum ATTACK_ACTIONS
    {
        CHEST_BITE = 0,
        CHEST_RUSH = 1,
        VIRUS_RUSH = 2,
        VIRUS_SPREAD = 3,
        SLIME_BITE = 4,
        SLIME_RUSH = 5,
        HORN_BITE = 6,
        HORN_RUSH= 7,
        DRAGON_BASIC=8,
        DRAGOND_CLAW_1 = 9,
        DRAGOND_CLAW_2 = 10,
        DRAGON_FLAME = 11,
        DRAGOND_ORBIT_FLAME = 12,
        NONE
    }

    public enum CONDITION_ACTIONS
    {
        VIRUS_SLOW = 0,
        VIRUS_HEAL = 1,
        NONE
    }
}

// 버프, 상태이상
namespace EffectEnums
{
    public enum HIT_EFFECTS
    {
        NONE = 0,
        KNOCKBACK = 1,
        STUN = 2,
        FALLDOWN=3,
    }

    /// <summary>
    /// Use With Multiplier
    /// </summary>
    public enum CONDITION_ATTRIBUTE_STATS
    {
        NONE = 0,
        MAXHP = 1,
        ATK = 2,
        DEF = 3,
    }

    public enum CONDITION_EFFECT_STATS
    {
        HP = 0,
        SPD = 1,
        ATK = 2,
        DEF = 3,
        ATKSPD = 4,
        DASH=100,
    }

    public enum CONDITION_APPLY_TYPE
    {
        VALUE = 0,
        OWN_PERCENT = 1, 
    }

    public enum CONDITION_CONTINUITY
    {
        IMMEDIATELY = 0, 
        BUFF = 1, 
        DEBUFF= 2
    }

    public enum CONDITION_PARTY
    {
        NONE,
        PARTY
    }

    public enum PARTICLES
    {
        BRUTE_ATK1,
        BRUTE_ATK2,
        BRUTE_ATK3,
        HEAL,
        HIT,
        DAMAGED,
        BUFF,
        NONE
    }
}

// Item
namespace ItemEnums
{
    public enum ITEMTYPE
    {
        ITEM_ETC = 0,
        ITEM_COMSUME = 1,
        ITEM_WEAPON = 2,
        ITEM_GOLD=3,
    }

    public enum WEAPONTYPE
    {
        WEAPON_KNUCKLE = 0,
        WEAPON_BOW = 1,
        WEAPON_STAFF = 2,
    }

    public enum WEAPONEFFECT
    {
        WEAPON_ATTACK = 0,
        WEAPON_CRITICAL = 1,
    }

    public enum ITEMID
    {
        GOLD=-1,

        // ETC
        LOW_CORE=0,
        MID_CORE=1,
        HIGH_CORE=2,

        // CONSUME
        HP_PORTION=100,
        ATTACK_PORTION=101,
        SPEED_PORTION=102,

        // WEAPON
        KNUCKLE=200,
        IRON_BLOOD_KNUCKLE=201,
        BOW=202,
        SONG_OF_WIND=203,
        STAFF=204,
        STARLIGHT_STAFF=205,
    }
}

#region ITEM

public enum CAMERAVIEW
{
    QUATERVIEW = 0,
    TALK =1,
    NONE=2,
}



#endregion

namespace QuestEnums
{
    public enum TYPES
    {
        ITEM,
        KILL,
        HELP,
    }
}