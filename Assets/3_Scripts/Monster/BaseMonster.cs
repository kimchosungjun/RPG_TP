using UnityEngine;
using MonsterEnums;

public abstract class BaseMonster : BaseCharacter
{
    [SerializeField] protected Animator anim = null;
    protected MonsterStatCtrl statCtrl;
    public override void SetCharacterType() 
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }

    /// <summary>
    /// 플레이어가 근처에 오면 상태창 활성화
    /// </summary>
    /// <returns></returns>
    public abstract BTS DetectPlayer();
    public abstract BTS IdleMovement();
    public abstract BTS DetectMovement();

    public abstract void Spawn();
    public abstract void Death();
    public abstract void TakeDamage();
    public abstract void Recovery();

    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void FixedUpdate() { } 
}