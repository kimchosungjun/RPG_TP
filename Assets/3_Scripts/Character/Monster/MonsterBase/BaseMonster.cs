using UnityEngine;
using MonsterEnums;
using System;
using System.Collections;
using UnityEngine.AI;
using MonsterTableClassGroup;

[RequireComponent(typeof(MonsterStatControl))]
public abstract class BaseMonster : BaseActor
{
    /******************************************/
    /**********      Component     ***********/
    /**********  NavMesh Agent  ***********/
    /******************************************/

    #region Value : Animator, StatControl, Navmesh, Audio
    [Header("Component"), SerializeField] protected Animator anim = null;
    [SerializeField] protected MonsterStatControl monsterStatControl;
    [SerializeField] protected NavMeshAgent nav;
    [SerializeField] protected MonsterFinder monsterFinder; 
    protected float toOriginalStopDistance = 0.5f;
    protected float toPlayerStopDistance = 1f;
    [SerializeField] protected MonsterStat monsterStat;
    public Animator GetAnim { get { return anim; } }    
    public MonsterStat GetMonsterStat { get { return monsterStat; } }
    [SerializeField] protected SFXPlayer sfxPlayer; 
    protected bool isDeathState = false;
    #endregion

    #region Value : Notice (Area)

    // Enemy Layer : Player
    protected int playerLayer = 1 << 8;

    // Area : Battle Field
    public bool IsInMonsterArea { get; protected set; } = false; 
    public BattleField MonsterArea { get; protected set; }
    public int BattleFieldSpawnIndex { get; protected set; } = -1;
    public Vector3 SpawnPosition { get; protected set; }
    public Vector3 FieldCenterPosition { get; set; }

    public void SetBattleFieldData(BattleField _field, int _fieldIndex, Vector3 _spawnPos, Vector3 _fieldPos) 
    {
        MonsterArea = _field;
        BattleFieldSpawnIndex = _fieldIndex;    
        SpawnPosition= _spawnPos;
        FieldCenterPosition = _fieldPos;
    }

    public void RespawnBattleFieldData(bool _isInArea)
    {
        IsInMonsterArea = _isInArea;
        if(_isInArea)
            AnnounceInMonsterArea();
        Revival();
    }

    #endregion

    #region Value : Monster Information
    [Header("Must Init"), Tooltip("Set Monster Type & Lv")]
    [SerializeField] protected InitMonsterData initMonsterData;
    public MonsterStatControl GetMonsterStatControl { get { return monsterStatControl; } }
    #endregion

    /******************************************/
    /**************  Set Layer  **************/
    /*************  Hit Method  **************/
    /******************************************/

    #region Override : Set Layer & Take Damage
    public override void SetDefaultLayerType()
    {
        this.gameObject.layer = (int)UtilEnums.LAYERS.MONSTER;
    }

    public override void SetCharacterType()
    {
        intLayer = (int)UtilEnums.LAYERS.MONSTER;
        bitLayer = 1 << (int)UtilEnums.LAYERS.MONSTER;
        characterTableType = UtilEnums.TABLE_FOLDER_TYPES.MONSTER;
    }
    public override bool CanTakeDamageState() { return true; }
    public override void ApplyStatTakeDamage(TransferAttackData _attackData) { monsterStatControl.TakeDamage(_attackData); }
    public override void ApplyMovementTakeDamage(TransferAttackData _attackData) { }
    public override void ApplyCondition(TransferConditionData _conditionData) { monsterStatControl.AddCondition(_conditionData); }
    #endregion

    /******************************************/
    /*************  Life Cycle  ***************/
    /******************************************/

    #region Virtual : Life Cycle

    /// <summary>
    /// Set Character Type, Link Monster Stat
    /// </summary>
    protected virtual void Awake()
    {
        SetCharacterType();
        if (monsterStatControl == null)
            monsterStatControl = GetComponent<MonsterStatControl>();
        if (nav == null)
            nav = GetComponent<NavMeshAgent>();
        nav.stoppingDistance = toPlayerStopDistance;
        monsterStatControl.BaseMonster = this;
    }

    /// <summary>
    /// Create BT States
    /// </summary>
    protected virtual void Start() 
    { 
        CreateStates();
        MonsterTable table = SharedMgr.TableMgr.GetMonster;
        MonsterInfoTableData infoTableData = table.GetMonsterInfoTableData(initMonsterData.monsterType);
        MonsterTableClassGroup.MonsterStatTableData statTableData = table.GetMonsterStatTableData(initMonsterData.monsterType);
        monsterStat = new MonsterStat();
        monsterStat.SetMonsterStat(statTableData, initMonsterData.monsterLevel);
        monsterStatControl.MonsterStat = monsterStat;
    }

    protected virtual void FixedUpdate() { }
    #endregion

    /******************************************/
    /*********** Virtual Method *************/
    /******************************************/

    #region Announce Area & Spawn 
    protected bool allPlayerDeath = false;
    public virtual void AnnounceInMonsterArea() { IsInMonsterArea = true; } 
    public virtual void AnnounceOutMonsterArea() { IsInMonsterArea = false; ReturnToSpawnPosition(); }
    public virtual void AnnounceAllPlayerDeath() { allPlayerDeath = true; }
    public virtual void AnnounceAllPlayerRevival() { allPlayerDeath = false; }
    public virtual void Death() { StopAllCoroutines(); anim.SetInteger("MState", (int)STATES.DEATH); SetNoneInteractionType(); GetDropItem(); } 
    public virtual void AfterDeath()  { MonsterArea.DeathMonster(this.gameObject);} 
    #endregion

    #region Return To Spawn Position
    public virtual void ReturnToSpawnPosition()
    {
        if (this.gameObject.activeSelf == false) return;

        GoOffAggro();
        Recovery();
    }

    public virtual void EscapeReturnToSpawnPosition()
    {
        if (isGoOffAggro == false || isRecovery == false) return;

        isGoOffAggro = false;
        isRecovery = false;
        nav.stoppingDistance = toPlayerStopDistance;
    }
    #endregion

    #region Go Off Aggro
    protected bool isGoOffAggro = false;
    public abstract void GoOffAggro();
    #endregion
    
    #region Recovery
    protected bool isRecovery = false;
    public virtual void Recovery(float _percent = 10f, float _time = 0.2f)
    {
        if (isRecovery) return;
        StartCoroutine(CRecovery(_percent, _time));
    }
    protected virtual IEnumerator CRecovery(float _percent, float _time)
    {
        float time = 0f;
        isRecovery = true;
        while (true)
        {
            if (isRecovery == false) yield break;
            if(monsterStatControl.CheckFullHp()) { isRecovery = false; yield break; }
            time+= Time.fixedDeltaTime;
            if(time >= _time)
            {
                time = 0f;
                monsterStatControl.Heal(_percent / 100, true);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region Must Override Methods
    protected abstract void CreateStates();
    public abstract void AnnounceStatusUI();
    public virtual void Revival() 
    {
        isDeathState = false;
        SetDefaultLayerType();
    }
    #endregion

    #region Drop

    public virtual void GetDropItem()
    {
        MonsterDropTableData dropData = SharedMgr.TableMgr.GetMonster.GetMonsterDropTableData(initMonsterData.monsterDropID);
        SharedMgr.QuestMgr.KillMonster((int)initMonsterData.monsterType);
        if (dropData == null)
            return;

        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.GOLD_SFX);
        int dropGold = dropData.dropGold;
        InventoryMgr inven = SharedMgr.InventoryMgr;

        // Gold
        if(dropGold > 0)
        {
            ItemData goldData = new ItemData(ItemEnums.ITEMTYPE.ITEM_GOLD, dropGold);
            inven.AddGold(dropGold);
            inven.ShowGetSlot(goldData);
        }

        // Exp
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.GetPlayerStatControl.GetExp(dropData.dropExp);

        // Item
        EtcData etcData = new EtcData();
        int dropItemTypeCnt = dropData.itemIDs.Length;
        // Set Quantity
        int quantity = 0;
        Randoms.GetQuantity(dropData.minQuantity, dropData.maxQuantity, dropData.quantityProbabilities, ref quantity);
        // Set Type
        int itemIndex = 0;
        Randoms.IsInProbability(dropData.itemDropProbabilities, ref itemIndex);
        etcData.SetData(SharedMgr.TableMgr.GetItem.
            GetEtcTableData(dropData.itemIDs[itemIndex]), quantity);
        // Add Inven
        inven.AddItem(etcData);
    }

    #endregion
}

#region Monster Lv, Type, DropID Class Data
[Serializable]
public class InitMonsterData
{
    public TYPEIDS monsterType;
    public int monsterLevel;
    public int monsterDropID;
}
#endregion