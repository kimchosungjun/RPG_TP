using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] EffectEnums.HIT_EFFECTS effect;
    /**********************************************/
    /************ 캐릭터 변경 변수 *************/
    /**********************************************/

    #region Value
    [Header("카메라 컨트롤러 : 필수 연결 요소"),SerializeField] CameraController cameraCtrl;
    [Header("동작하는 캐릭터 종류"), SerializeField] List<BasePlayer> players;
    [Header("파티 버프 관리"), SerializeField] PartyConditionControl partyConditionControl;
    bool canChangePlayer = true; // when start : must init
    float changePlayerCoolTime = 1f; 
    [SerializeField] int currentPlayer = 0; // when start : must init

    public BasePlayer GetPlayer { get { return players[currentPlayer]; } }
    public List<BasePlayer> GetPlayers { get { return players; } }
    #endregion

    #region Life Cycle
    private void Awake() 
    {
        InitPartyData(players);
    }

    private void Start() 
    {
       SetPartyData(players);
    }

    private void Update()
    {
        players[currentPlayer].Execute();
#if UNITY_EDITOR
        InputChangeKey();
#endif
    }

    private void FixedUpdate() 
    {
        players[currentPlayer].FixedExecute(); 
        partyConditionControl.FixedExecute();
    }
    #endregion

    /**********************************************/
    /************ 플레이어 변경 ****************/
    /**********************************************/

    #region Change Player 

    public void InputChangeKey()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangePlayer(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangePlayer(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangePlayer(2);
    }

    public void DeathChangePlayer()
    {
        int partyCnt = players.Count;
        for (int i = 0; i < partyCnt; i++)
        {
            if (players[i].GetIsAlive)
            {
                cameraCtrl.QuaterViewChangeTarget(players[i].GetPlayerMovementControl.GetBodyTransform);
                ChangePlayer(i, false);
            }
        }

        // To Do ~~~~~
        // 죽음 UI를 보여줘야 한다.
        Debug.Log("모든 플레이어가 사망..");
    }

    public void ChangePlayer(int _index, bool _checkCoolTime = true)
    {
        // 할당되지 않는 파티원을 불러올 순 없다.
        int partyCnt = players.Count-1;
        if (_index > partyCnt) return;

        if (currentPlayer == _index) return;

        // 쿨타임과 죽은 상태일때는 바로 return; => UI 추가해야 함
        if (_checkCoolTime && canChangePlayer == false) return;
        if (players[_index].GetIsAlive == false) return;

        players[currentPlayer].gameObject.SetActive(false);
        cameraCtrl.QuaterViewChangeTarget(players[_index].GetPlayerMovementControl.GetBodyTransform);
        players[_index].transform.position = players[currentPlayer].transform.position;
        players[_index].transform.rotation = players[currentPlayer].transform.rotation;
        players[_index].gameObject.SetActive(true);
        currentPlayer = _index;

        canChangePlayer = false;
        partyConditionControl.ChangePlayer(players[currentPlayer].PlayerStat);
        SharedMgr.UIMgr.GameUICtrl.GetPlayerChangeUI.SetCoolTime(changePlayerCoolTime, CoolDown);
    }

    public void CoolDown() { canChangePlayer = true; }
    #endregion


    /**********************************************/
    /*************** 파티 변경 ******************/
    /**********************************************/

    #region Change Party

    public bool CanChangeParty(List<BasePlayer> _newParty)
    {
        int newPartyCnt = _newParty.Count;
        for (int i = 0; i < newPartyCnt; i++)
        {
            if (_newParty[i].GetIsAlive)
                return true;
        }
        return false;
    }

    public void ChangeParty(List<BasePlayer> _newParty)
    {
        players = _newParty;
        int newPartyCnt = _newParty.Count;
        for(int i = 0;i < newPartyCnt;i++)
        {
            if (_newParty[i].GetIsAlive)
            {
                currentPlayer = i;
                InitPartyData(_newParty);
                SetPartyData(_newParty);
                players = _newParty;
                int legacyPartyCnt = players.Count;
                for(int k=0; k< legacyPartyCnt; k++)
                {
                    players[k].gameObject.SetActive(false); 
                }
                return;
            }
        }
    }

    #endregion


    /**********************************************/
    /*************** 파티 설정 ******************/
    /**********************************************/
    #region Set Player Data
    public void InitPartyData(List<BasePlayer> _party)
    {
        int cnt = _party.Count;
        for(int i = 0; i < cnt; i++)
        {
            _party[i].Init();
        }
    }

    public void SetPartyData(List<BasePlayer> _party)
    {
        int cnt = _party.Count;
        for (int i = 0; i < cnt; i++)
        {
            _party[i].Setup();
            if (currentPlayer == i)
            {
                cameraCtrl.QuaterViewChangeTarget(_party[i].GetPlayerMovementControl.GetBodyTransform);
                _party[i].gameObject.SetActive(true); 
            }
            else
                _party[i].gameObject.SetActive(false); 
        }
    }

    #endregion

    #region Player Dash Gauge
    float dashGauge = 1f;
    bool showDashGauge = false;
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
            SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.UseDashGauge();
        
    }

    IEnumerator CRecoveryDashGauge()
    {
        bool isInActiveUI = false;
        while (true)
        {
            dashGauge += increaseDashGaugePerSecond * Time.deltaTime; 
            SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.SetGaugeAmount(dashGauge);
            if (dashGauge >=0.99f)
                break;

            if(isInActiveUI == false && Time.time - lastDashTime > 3f )
            {
                SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.InActiveGauge();
                isInActiveUI = true;
            }
            yield return null;
        }

        if(isInActiveUI == false)
            SharedMgr.UIMgr.GameUICtrl.GetDashGaugeUI.InActiveGauge();
        showDashGauge = false;
    }

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
}
