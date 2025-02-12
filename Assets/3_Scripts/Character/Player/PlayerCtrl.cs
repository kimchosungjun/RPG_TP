using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    /**********************************************/
    /************ 캐릭터 변경 변수 *************/
    /**********************************************/

    #region Value
    [SerializeField] bool isLockPlayerControl = false;
    
    [Header("동작하는 캐릭터 종류"), SerializeField] List<BasePlayer> players;
    [Header("파티 버프 관리"), SerializeField] PartyConditionControl partyConditionControl;
    bool canChangePlayer = true; // when start : must init
    float changePlayerCoolTime = 1f; 
    [SerializeField] int currentPlayerIndex = 0; // when start : must init
    public int GetCurrentPlayerIndex { get { return currentPlayerIndex; } }
    public BasePlayer GetPlayer { get { return players[currentPlayerIndex]; } }
    public List<BasePlayer> GetPlayers { get { return players; } }

    public bool CanInteractUI() { return GetPlayer.CanInteractUI(); }
    #endregion

    #region Life Cycle

    private void Awake()
    {
        InitPartyData();
    }

    private void Start()
    {
        SetPartyData(players);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetQuaterView(players[currentPlayerIndex].GetPlayerMovementControl.GetBodyTransform);
    }

    private void Update()
    {
        if (isLockPlayerControl || SharedMgr.UIMgr.GameUICtrl.CanControlPlayer() == false) return;
        players[currentPlayerIndex].Execute();
        InputChangeKey();
    }

    private void FixedUpdate() 
    {
        if (isLockPlayerControl || SharedMgr.UIMgr.GameUICtrl.CanControlPlayer() ==false) return;
        players[currentPlayerIndex].FixedExecute(); 
        partyConditionControl.FixedExecute();
    }
    #endregion

    /**********************************************/
    /************ 플레이어 변경 ****************/
    /**********************************************/

    #region Change Player 

    public void InputChangeKey()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangePlayer(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangePlayer(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangePlayer(2);
#endif
    }

    public void DeathChangePlayer()
    {
        int partyCnt = players.Count;
        for (int i = 0; i < partyCnt; i++)
        {
            if (players[i].IsAlive)
            {
                SharedMgr.GameCtrlMgr.GetCameraCtrl.SetQuaterView(players[i].GetPlayerMovementControl.GetBodyTransform);
                ChangePlayer(i, false);
                return;
            }
        }
        SharedMgr.UIMgr.GameUICtrl.GetIndicatorUI.ActiveGameOver();
        SetPlayerControl(true);
    }

    public void PressChangePlayer(int _index) { ChangePlayer(_index); }

    public void ChangePlayer(int _index, bool _checkCoolTime = true)
    {
        // 할당되지 않는 파티원을 불러올 순 없다.
        int partyCnt = players.Count-1;
        if (_index > partyCnt) return;

        if (currentPlayerIndex == _index) return;

        PlayerChangeUI changeUI = SharedMgr.UIMgr.GameUICtrl.GetPlayerChangeUI;
        if (_checkCoolTime && canChangePlayer == false) 
        {
            changeUI.ShowWarnText(UIEnums.CHANGE.COOLDOWN);
            return;
        }
        if (_checkCoolTime && players[_index].IsAlive == false)
        {
            changeUI.ShowWarnText(UIEnums.CHANGE.DEATH);
            return;
        }
        if(_checkCoolTime == false)
        {
            players[currentPlayerIndex].InitState();
        }
        else
        {
            if (players[currentPlayerIndex].GetCanChangeState == false)
            {
                changeUI.ShowWarnText(UIEnums.CHANGE.CANNOTCHANGE);
                return;
            }
        }

        players[currentPlayerIndex].InitState();
        players[currentPlayerIndex].gameObject.SetActive(false);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetQuaterView(players[_index].GetPlayerMovementControl.GetBodyTransform);
        players[_index].SetTransform(players[currentPlayerIndex].transform.position, players[currentPlayerIndex].transform.rotation, 
            players[currentPlayerIndex].GetPlayerMovementControl.GetRigid.velocity);
        players[_index].gameObject.SetActive(true);
        currentPlayerIndex = _index;

        canChangePlayer = false;
        SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.ChangeData(players[currentPlayerIndex].PlayerStat);
        partyConditionControl.ChangePlayer(players[currentPlayerIndex].PlayerStat);
        changeUI.ControlButtonEffect(currentPlayerIndex);
        changeUI.SetCoolTime(changePlayerCoolTime, CoolDown);
    }

    public void CoolDown() { canChangePlayer = true; }
    #endregion

    /**********************************************/
    /*************** 파티 설정 ******************/
    /**********************************************/

    #region Set Player Data
    public void InitPartyData()
    {
        List<int> playerIDSet = SharedMgr.SaveMgr.GetUserSaveData.PlayerSaveDataGroup.CurrentPlayerPartyIDSet;
        int idSetCnt = playerIDSet.Count;
        List<BasePlayer> basePlayers = new List<BasePlayer>();
        Vector3 savePosition = SharedMgr.SaveMgr.GetUserSaveData.PlayerSaveDataGroup.CurrentPlayerPosition;
        Quaternion saveRotation = SharedMgr.SaveMgr.GetUserSaveData.PlayerSaveDataGroup.GetPlayerRotation();
        for (int i=0; i<idSetCnt; i++)
        {
            GameObject playerObject = Instantiate(SharedMgr.ResourceMgr.GetBasePlayer
                (SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(playerIDSet[i]).prefabName).gameObject);
            playerObject.transform.SetParent(this.transform, false);
            playerObject.transform.position = savePosition;
            playerObject.transform.rotation = saveRotation; 
            if(playerObject.activeSelf==false) playerObject.SetActive(true);    
            BasePlayer basePlayer = playerObject.GetComponent<BasePlayer>();
            basePlayer.PlayerID = playerIDSet[i];
            basePlayers.Add(basePlayer);    
        }
        players = basePlayers;
        int cnt = players.Count;
        for(int i = 0; i < cnt; i++)
        {
            players[i].Init();
        }
        SharedMgr.UIMgr.GameUICtrl.GetPlayerChangeUI.SetButtonData(currentPlayerIndex);
    }

    public void SetPartyData(List<BasePlayer> _party)
    {
        int cnt = _party.Count;
        for (int i = 0; i < cnt; i++)
        {
            _party[i].Setup();
            if (currentPlayerIndex == i)
            {
                _party[i].gameObject.SetActive(true);
                SharedMgr.GameCtrlMgr.GetCameraCtrl.SetQuaterView(_party[i].GetPlayerMovementControl.GetBodyTransform);
                partyConditionControl.SetPlayerStat(_party[i].GetPlayerStatControl.PlayerStat);
                SharedMgr.UIMgr.GameUICtrl.GetPlayerStatusUI.ChangeData(_party[i].GetPlayerStatControl.PlayerStat);
            }
            else
                _party[i].gameObject.SetActive(false); 
        }
    }

    #endregion

    #region Join Party 
    public void JoinParty(int _characterID)
    {
        int partyCnt = players.Count;
        if (partyCnt > 3)
            return;

        for(int i=0; i<partyCnt; i++)
        {
            if (players[i].PlayerID == _characterID)
                return;
        }
        GameObject playerObject = Instantiate(SharedMgr.ResourceMgr.GetBasePlayer
               (SharedMgr.TableMgr.GetPlayer.GetPlayerTableData(_characterID).prefabName).gameObject);
        playerObject.transform.SetParent(this.transform, false);
        playerObject.transform.position = GetPlayer.transform.position;
        playerObject.transform.rotation = GetPlayer.transform.rotation;
        BasePlayer basePlayer = playerObject.GetComponent<BasePlayer>(); 
        basePlayer.PlayerID = _characterID;
        players.Add(basePlayer);
        basePlayer.Init();
        basePlayer.Setup();
        SharedMgr.UIMgr.GameUICtrl.GetPlayerChangeUI.SetButtonData(currentPlayerIndex);
    }
    #endregion

    #region Player Dash Gauge
    float dashGauge = 1f;
    bool showDashGauge = false;
    bool isInActiveUI = true;
    float lastDashTime = 0f;
    [SerializeField] float increaseDashGaugePerSecond = 0.05f;
    public bool CanDash()
    {
        if (dashGauge - 0.2f > 0f)
            return true;
        return false;
    }

    public void DoDash()
    {
        dashGauge -= 0.2f;
        lastDashTime = Time.time;
        if (showDashGauge == false)
        {
            showDashGauge = true;
            StartCoroutine(CRecoveryDashGauge());
        }
        else
        {
            isInActiveUI = false;
            SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.UseDashGauge();
        }
        
    }

    IEnumerator CRecoveryDashGauge()
    {
        DashGaugeUI dashGaugeUI = SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI;
        if (dashGaugeUI == null) yield break;
        isInActiveUI = false;
        dashGaugeUI.ActiveGauge();
        while (true)
        {
            dashGauge += increaseDashGaugePerSecond * Time.deltaTime;
            dashGaugeUI.SetGaugeAmount(dashGauge);
            if (dashGauge >= 0.99f)
            {
                dashGaugeUI.SetGaugeAmount(1f);
                dashGaugeUI.InActiveGauge();
                break;
            }

            if(isInActiveUI == false && Time.time - lastDashTime > 10f )
            {
                dashGaugeUI.InActiveGauge();
                isInActiveUI = true;
            }
            yield return null;
        }

        if(isInActiveUI == false)
            dashGaugeUI.InActiveGauge();
        showDashGauge = false;
    }

    // Debuff 
    public void DecreaseDashGauge(float _dashGaugePercent)
    {
        float decreaseValue = dashGauge - _dashGaugePercent > 0f ? _dashGaugePercent: dashGauge - _dashGaugePercent;
        lastDashTime = Time.time;
        if (showDashGauge == false)
        {
            showDashGauge = true;
            StartCoroutine(CRecoveryDashGauge());
        }
        else
            SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.UseDashGauge(decreaseValue);
       
    }

    #endregion

    #region Heal All Player

    public void HealAllPlayer()
    {
        int cnt = players.Count;
        for(int i=0; i<cnt; i++)
        {
            players[i].GetPlayerStatControl.InHealField();
        }
    }

    public void MoveToNearTown()
    {
        int cnt = players.Count;
        for (int i = 0; i < cnt; i++)
        {
            players[i].GetPlayerStatControl.InHealField();
        }
        players[currentPlayerIndex].transform.position = new Vector3(212, 0.1f, 153);
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetMoveRockCamera(false);
        StartCoroutine(CReleaseDeathState());
    }

    IEnumerator CReleaseDeathState() { yield return new WaitForSeconds(1f); SharedMgr.UIMgr.GameUICtrl.GetIndicatorUI.FadeIn(ReleaseMoveLock); }

    #endregion

    /**********************************************/
    /*************** 상태 설정 ******************/
    /**********************************************/

    #region Conversation Control
    public void StartConversation(Vector3 _targetPosition) { players[currentPlayerIndex].GetPlayerMovementControl.StartConversation(_targetPosition); }
    public void EndConversation() { players[currentPlayerIndex].GetPlayerMovementControl.EndConversation(); }

    #endregion

    #region Character Move Control

    public void SetPlayerControl(bool _isLockPlayerControl)
    {
        isLockPlayerControl = _isLockPlayerControl;
        SharedMgr.GameCtrlMgr.GetCameraCtrl.SetMoveRockCamera(_isLockPlayerControl);
    }

    public void ReleaseMoveLock() { SetPlayerControl(false); }

    #endregion


    /**********************************************/
    /*************** 소리 설정 ******************/
    /**********************************************/
    //public void FootStep() { footStepPlayer.SoundFootStep(); }
}


/**********************************************/
/*************** 파티 변경 ******************/
/************ 현재 버전 사용 X *************/
/**********************************************/

#region Change Party : Not Use

//public bool CanChangeParty(List<BasePlayer> _newParty)
//{
//    int newPartyCnt = _newParty.Count;
//    for (int i = 0; i < newPartyCnt; i++)
//    {
//        if (_newParty[i].IsAlive)
//            return true;
//    }
//    return false;
//}

//public void ChangeParty(List<BasePlayer> _newParty)
//{
//    players = _newParty;
//    int newPartyCnt = _newParty.Count;
//    for (int i = 0; i < newPartyCnt; i++)
//    {
//        if (_newParty[i].IsAlive)
//        {
//            currentPlayerIndex = i;
//            InitPartyData(_newParty);
//            SetPartyData(_newParty);
//            players = _newParty;
//            int legacyPartyCnt = players.Count;
//            for (int k = 0; k < legacyPartyCnt; k++)
//            {
//                players[k].gameObject.SetActive(false);
//            }
//            return;
//        }
//    }
//}

#endregion