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
        RAYFISH=0,
        FLYBEE=1,
        SLIME=2,
        VIRUS=3,
        ANNOYBEAR=4,
        BULLTANK=5,
        DRAGON=6
    }

    public enum ATTACK_ACTIONS
    {
        SLIME_RUSH=0,
        VIRUS_RUSH=1,
        VIRUS_SPREAD=2,
        BEAR_SMASH01=3,
        BEAR_SMASH02=4,
        BEAR_SMASH03=5,
        BEAR_STAMP=6
    }

    public enum CONDITION_ACTIONS
    {
        VIRUS_SLOW=0,
        VIRUS_HEAL=1,
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
    /// 계수와 같이 사용
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
        SPEED=5,
    }

    public enum CONDITION_CONTINUITY
    {
        IMMEDIATELY = 0, 
        BUFF = 1, 
        DEBUFF= 2 // 즉발 디버프는 없음
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

// To Do ~~~~~
// 아이템
namespace ItemEnums
{
    public enum ITEMTYPE
    {
        ITEM_NONE = 0,
        ITEM_ETC = 1,
        ITEM_COMSUMPTION = 1,
        ITEM_WEAPON = 2,
    }

    public enum WEAPONTYPE
    {
        WEAPON_NONE = 0,
        WEAPON_SWORD = 1,
        WEAPON_BOW = 2,
        WEAPON_STAFF = 3,
    }

    public enum WEAPONEFFECT
    {
        WEAPON_ATTACK = 0,
        WEAPON_CRITICAL = 1,
    }
}

#region ITEM

public enum E_CAMERAVIEW
{
    CAMERA_QUATERVIEW = 0,
    CAMERA_CLOSEUP = 1,
}



#endregion