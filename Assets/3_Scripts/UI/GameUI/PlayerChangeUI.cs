using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerChangeUI : UIBase
{

    [SerializeField] PlayerChangeButton[] buttons;
    [SerializeField] GameObject uiFrameParent;
    [SerializeField] ChangeWarnWindow warnWindow;

    /******************************************/
    /**************  Methods  ***************/
    /******************************************/

    #region Set Button Data
    public void Init()
    {
        TurnOff();
        SetImages();
    }

    public void SetButtonData(int _currentPlayerIndex)
    {
        List<BasePlayer> players = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers;
        int playerCnt = players.Count - 1;
        int buttonCnt = buttons.Length;
        PlayerTable playerTable = SharedMgr.TableMgr.GetPlayer;
        for (int i = 0; i < buttonCnt; i++)
        {
            if (i > playerCnt)
                buttons[i].gameObject.SetActive(false);
            else
            {
                buttons[i].ChangeButtonData(players[i].PlayerStat);
                if (_currentPlayerIndex == i)
                    buttons[i].ControlEffect(true);
                else
                    buttons[i].ControlEffect(false);
                if (buttons[i].gameObject.activeSelf == false)
                    buttons[i].gameObject.SetActive(true);
            }
        }
        TurnOn();
    }
    #endregion

    #region CoolDown
    public void SetCoolTime(float _coolTime, UnityAction _announceCoolDown)
    {
        StartCoroutine(CChangeCoolDown(_coolTime, _announceCoolDown));
    }

    IEnumerator CChangeCoolDown(float _coolTime, UnityAction _announceCoolDown)
    {
        float time = 0f;
        int cnt = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers.Count;

        int curPlayer = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetCurrentPlayerIndex;
        for (int i = 0; i < cnt; i++)
        {
            buttons[i].GetPanel().fillAmount = 1f;
            buttons[i].GetPanel().gameObject.SetActive(true);

            if (curPlayer == i)
                buttons[i].ControlEffect(true);
            else
                buttons[i].ControlEffect(false);

        }

        while (time < _coolTime)
        {
            time += Time.fixedDeltaTime;
            for (int i = 0; i < cnt; i++)
            {
                buttons[i].GetPanel().fillAmount = Mathf.Lerp(1, 0, time / _coolTime);
                yield return new WaitForFixedUpdate();
            }
        }

        for (int i = 0; i < cnt; i++)
        {
            buttons[i].GetPanel().gameObject.SetActive(false);
        }

        if (_announceCoolDown != null)
            _announceCoolDown.Invoke();
    }

    public void ShowWarnText(UIEnums.CHANGE _changeType)
    {
        if (warnWindow.gameObject.activeSelf == false)
            warnWindow.gameObject.SetActive(true);
        warnWindow.ShowWarnText(_changeType);
    }

    #endregion

    #region Window Ver 
    public void ControlButtonEffect(int _index)
    {
        int cnt = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers.Count;
        for (int i = 0; i < cnt; i++)
        {
            if (i == _index)
                buttons[_index].ControlEffect(true);
            else
                buttons[_index].ControlEffect(false);
        }
    }


    #endregion

    #region Update Button State
    public void UpdateChangeButton()
    {
        int cnt = buttons.Length;
        for(int i=0; i<cnt; i++)
        {
            buttons[i].UpdateLiveState();
        }
    }

    public void UpdateAliveButton()
    {
        int cnt = buttons.Length;
        for (int i = 0; i < cnt; i++)
        {
            buttons[i].AliveState();
        }
    }

    public void LevelUp()
    {
        int cnt = buttons.Length;
        for (int i = 0; i < cnt; i++)
        {
            buttons[i].UpdateLevel();
        }
    }


    #endregion

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface

    public override void TurnOn()
    {
        uiFrameParent.SetActive(true);
    }

    public override void TurnOff()
    {
        warnWindow.gameObject.SetActive(false);
        uiFrameParent.SetActive(false);
    }

    public void SetImages()
    {
        int cnt = buttons.Length;
        for (int i = 0; i < cnt; i++)
        {
            buttons[i].SetImage();
            buttons[i].EmptyButton();
        }
        warnWindow.SetImage();
    }
    #endregion

}
