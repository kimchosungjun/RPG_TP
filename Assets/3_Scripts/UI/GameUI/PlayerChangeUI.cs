using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerChangeUI : MonoBehaviour
{
    bool isActive = true;
    [SerializeField] PlayerChangeButton[] buttons;
    [SerializeField] GameObject uiFrameParent;

    #region Set Button Data
    public void Init()
    {
        int cnt = buttons.Length;   
        for(int i=0; i<cnt; i++)
        {
            buttons[i].SetImage();
            buttons[i].EmptyButton();
        }

        if(uiFrameParent.activeSelf==false)
            uiFrameParent.SetActive(true);
    }

    public void SetButtonData()
    {
        List<BasePlayer> players = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers;
        int playerCnt = players.Count -1;
        int buttonCnt = buttons.Length;

        for(int i=0; i<buttonCnt; i++)
        {
            if (i > playerCnt)
                buttons[i].gameObject.SetActive(false);
            else
                buttons[i].ChangeButtonData(players[i].PlayerStat.GetSaveStat.currentLevel, players[i].gameObject.name);
        }
    }
    #endregion

    #region CoolDown
    public void SetCoolTime(float _coolTime, UnityAction _announceCoolDown)
    {
        if (isActive == false) return;

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

        while (time <_coolTime)
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
    #endregion

    #region Control UI Active State
    public void Active()
    {
        isActive = true;
        uiFrameParent.SetActive(true);
    }

    public void InActive()
    {
        isActive = false;
        uiFrameParent.SetActive(false);
    }
    #endregion

    #region Window Ver 
    public void ControlButtonEffect(int _index)
    {
        int cnt = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers.Count;
        for(int i=0; i<cnt; i++)
        {
            if(i == _index)
                buttons[_index].ControlEffect(true);
            else
                buttons[_index].ControlEffect(false);
        }
    }
    #endregion
}
