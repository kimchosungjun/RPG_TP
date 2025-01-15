using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSlotSetUI : MonoBehaviour
{
    [SerializeField] PlayerPartyStatusButton[] buttons;
    [SerializeField] GameObject slotSetParent;
    public void Init()
    {
        int btnCnt = buttons.Length;
        for(int i=0; i<btnCnt; i++)
        {
            buttons[i].Init();
        }

        if (slotSetParent == null)
            slotSetParent = transform.GetChild(1).GetComponent<GameObject>();
    }

    public void TurnOn()
    {
        UpdateCharacterButton();
        slotSetParent.SetActive(true);
    }

    public void TurnOff()
    {
        if (slotSetParent.activeSelf == false) return;
        slotSetParent.SetActive(false);
    }

    public void UpdateCharacterButton()
    {
        PlayerStat stat = null;
        ResourceMgr res = SharedMgr.ResourceMgr;
        List<BasePlayer> players = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayers;
        int slotCnt = buttons.Length;
        int playerCnt = players.Count;
        for (int i = 0; i < playerCnt; i++)
        {
            if (i > slotCnt - 1) break;
            stat = players[i].PlayerStat;
            int id = stat.GetSaveStat.playerTypeID;
            buttons[i].SetButton(id, res.GetSpriteAtlas(stat.GetAtlasName, stat.GetFileName));
        }
        AnnounceFirstChracter();
    }

    public void AnnounceFirstChracter()
    {
        buttons[0].PressUnderCharacterButton();
    }
}
