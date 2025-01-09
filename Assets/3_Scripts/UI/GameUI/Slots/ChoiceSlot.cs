using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChoiceSlot : MonoBehaviour
{
    [SerializeField] Text choiceText;
    [SerializeField, Header("0 : F Key, 1 : Chat Icon, 2 : Direction Icon, 3:Choice Frame")] Image[] slotImages;

    int slotEventID = -1;
    public int SlotEventID { get { return slotEventID; }set { slotEventID = value; } }
    Choice choice = null;
    UnityAction<int> choiceAction = null;
    public UnityAction<int> ChoiceAction { get { return choiceAction; } set { choiceAction = value; } }


    public void Active(Choice _choice)
    {
        choice = _choice;
        choiceText.text = SharedMgr.InteractionMgr.GetDialogueReader.ReadChoiceText(_choice.choiceTexts, this);
        this.gameObject.SetActive(true);
    }

    public void InActive()
    {
        if (this.gameObject.activeSelf == true)
        {
            choice = null;
            this.gameObject.SetActive(false);
        }
    }
    
    public void PressSlot()
    {
        if(choiceAction !=null)
            choiceAction(slotEventID);

        if (choice.continueDialouge)
        {
            SharedMgr.InteractionMgr.ContinueConversation(choice.nextDialogueID);
        }
        else
        {
            SharedMgr.InteractionMgr.EndConversation();
        }
    }

    public void ActiveDirection()
    {
        slotImages[2].gameObject.SetActive(true);
    }

    public void InActiveDirection()
    {
        slotImages[2].gameObject.SetActive(false);
    }

    public void SetImage()
    {
        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "FKey_Icon");
        slotImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Chat_Icon");
        slotImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Direction_Icon");
        slotImages[3].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Dialogue_Bar");
    }
}
