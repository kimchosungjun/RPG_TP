using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    /******************************************/
    /****************  Value  *****************/
    /******************************************/

    #region UI
    [SerializeField] GameObject conversationUIParentObject;
    [SerializeField] ChoiceSlot[] choiceSlots;
    [SerializeField, Header("0:Name, 1:Dialogue")] Text[] dialogueTexts;
    [SerializeField] Image textDivisionImage;
    #endregion

    #region Dialogue Data
    int currentDialogueIndex = 0;
    bool haveEvent = false;
    Dialogue dialogue = null;
    bool isChoiceActive = false;
    public bool GetIsChoiceActive { get { return isChoiceActive; } }
    [SerializeField] int currentChoiceIndex = 0;
    [SerializeField] int activeSlotCnt = 0;
    #endregion

    #region Type
    bool isTypeText = false;
    string currentText = string.Empty;
    public bool SetAutoMode { set { isAuto = value; } }   
    [SerializeField] bool isAuto = false;
    [SerializeField, Header("Text Speed"), Range(0.2f, 5f)] float typeSpeed;
    [SerializeField, Header("Delay Speed"), Range(0.2f, 1f)] float delayNextDialougeSpeed;
    #endregion

    /******************************************/
    /***************  Method ****************/
    /******************************************/

    #region Show Dialogue
    public void StartConversation(Dialogue _dialogue, bool _isContinueConversation = false)
    {
        if (_dialogue == null) return;
        
        ActiveUI();

        dialogueTexts[0].text = _dialogue.speakerName;
        if (_dialogue.storyLines.Count == 0)
            ShowChoiceDialogue(_dialogue.choiceLines);
        else
        {
            currentDialogueIndex = 0;
            dialogue = _dialogue;
            ShowDialogue();
        }
    }
    
    public void ActiveUI()
    {
        CloseAllChoiceSlots();
        ClearDialogueTexts();
        conversationUIParentObject.SetActive(true);
    }
    
    public void ShowDialogue()
    {
        if(currentDialogueIndex >= dialogue.storyLines.Count)
        {
            ShowChoiceDialogue(dialogue.choiceLines);
            return;
        }
        string text  = SharedMgr.InteractionMgr.GetDialogueReader.ReadText(dialogue.storyLines[currentDialogueIndex], out haveEvent);
        if (haveEvent)
        {
            currentDialogueIndex += 1;
            ShowDialogue();
        }
        else
            StartCoroutine(CTypeDialogueText(text));
    }

    public void ShowChoiceDialogue(List<Choice> _choiceLines)
    {
        if(_choiceLines == null || _choiceLines.Count == 0)
        {
            EndConversation();  
            return;
        }

        isChoiceActive = true;
        int choiceCnt = _choiceLines.Count;

        ActiveDirectionIndicator(choiceCnt);
        for(int i=0; i<choiceCnt; i++)
        {
            choiceSlots[i].Active(_choiceLines[i]);
        }
    }
    #endregion

    #region Type Dialogue
    IEnumerator CTypeDialogueText(string _text)
    {
        currentText = _text;
        isTypeText = true;
        int _textCnt = _text.Length;
        string text = string.Empty;
        for(int i=0; i<_textCnt; i++)
        {
            text+= _text[i];
            dialogueTexts[1].text = text;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTypeText = false;
        currentDialogueIndex += 1;
        if (isAuto)
        {
            yield return new WaitForSeconds(delayNextDialougeSpeed);
            ShowDialogue();
        }
    }

    public void SkipText()
    {
        if (isTypeText)
            StopAllCoroutines();

        dialogueTexts[1].text = currentText;
        isTypeText = false;
    }
    #endregion

    #region Maintain Conversation
    public void ContinueConversation(Dialogue _dialogue)
    {
        ClearDialogueTexts();
        CloseAllChoiceSlots();
        isChoiceActive = false;
        StartConversation(_dialogue);
    }
    public void ClearDialogueTexts()
    {
        for (int i = 0; i < 2; i++)
        {
            dialogueTexts[i].text = string.Empty;
        }
    }

    public void EndConversation()
    {
        CloseAllChoiceSlots();
        isChoiceActive = false;
        conversationUIParentObject.SetActive(false);
    }

    public void CloseAllChoiceSlots()
    {
        int choiceCnt = choiceSlots.Length;
        for (int i = 0; i < choiceCnt; i++)
        {
            choiceSlots[i].InActive();
        }
    }
    #endregion

    #region Input Up & Down Key : Only Use Window Ver
    public void ActiveDirectionIndicator(int _slotCnt)
    {
        currentChoiceIndex = 0;
        activeSlotCnt = _slotCnt;
        ClearDirectionIndicator();
    }

    public void InputUpKey()
    {
        if (currentChoiceIndex == 0)
            return;
        choiceSlots[currentChoiceIndex].InActiveDirection();
        currentChoiceIndex -= 1;
        choiceSlots[currentChoiceIndex].ActiveDirection();
    }

    public void InputDownKey()
    {
        if (currentChoiceIndex == activeSlotCnt - 1)
            return;
        choiceSlots[currentChoiceIndex].InActiveDirection();
        currentChoiceIndex += 1;
        choiceSlots[currentChoiceIndex].ActiveDirection();
    }

    public void ClearDirectionIndicator()
    {
        choiceSlots[currentChoiceIndex].ActiveDirection();
        int slotCnt = choiceSlots.Length;
        for (int i = 1; i < slotCnt; i++)
        {
            choiceSlots[i].InActiveDirection();
        }
    }

    public void SelectChoice()
    {
        choiceSlots[currentChoiceIndex].PressSlot();
    }
    #endregion
}
