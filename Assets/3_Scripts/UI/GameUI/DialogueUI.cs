using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour, ICommonSetUI
{
    /******************************************/
    /****************  Value  *****************/
    /******************************************/

    #region UI
    [SerializeField] GameObject conversationUIParentObject;
    [SerializeField] ChoiceSlot[] choiceSlots;
    [SerializeField, Header("0:Name, 1:Dialogue")] Text[] dialogueTexts;
    [SerializeField] Image textDivisionImage;
    [SerializeField] ConversationAutoButton autoButton;
    [SerializeField] GameObject dialogueFrame;
    #endregion

    #region Dialogue Data
    int curContentIndex = 0;
    int curStoryLineIndex = 0;
    bool haveEvent = false;
    Dialogue dialogue = null;
    bool isChoiceActive = false;
    public bool GetIsChoiceActive { get { return isChoiceActive; } }
    int currentChoiceIndex = 0;
    int activeSlotCnt = 0;
    #endregion

    #region Type
    bool isWaitingInput = false;
    bool isTypeText = false;
    string currentText = string.Empty;
    
    [SerializeField,Header("Auto Mode")] bool isAuto = false;
    
    float typeSpeed = 0.125f;
    WaitForSeconds typeSecond = new WaitForSeconds(0.125f);
    WaitForSeconds delayAutoDialogueSecond = new WaitForSeconds(1f);

    public void SetLowTypeSpeed() { typeSpeed = 0.2f; typeSecond = new WaitForSeconds(typeSpeed); }
    public void SetMidTypeSpeed() { typeSpeed = 0.125f; typeSecond = new WaitForSeconds(typeSpeed); }
    public void SetHighTypeSpeed() { typeSpeed = 0.05f; typeSecond = new WaitForSeconds(typeSpeed); }
    #endregion


    /******************************************/
    /***************  Method ****************/
    /******************************************/

    public void Init()
    {
        SetImages();
    }

    #region Show Dialogue
    public void StartConversation(Dialogue _dialogue, bool _isContinueConversation = false)
    {
        if (_dialogue == null) return;
        ActiveUI();
        dialogue = _dialogue;
        if (autoButton.gameObject.activeSelf == false)
            autoButton.gameObject.SetActive(true);
        autoButton.OnOffEffect(isAuto);
        dialogueTexts[0].text = dialogue.speakerName;
        SetConversation();
    }

    public void ReverseAutoButton()
    {
        isAuto = !isAuto;
        autoButton.OnOffEffect(isAuto);

        if (isWaitingInput && isAuto)
            ShowDialogue();
    }

    public void SetConversation(int _curContentIndex = 0)
    {
        if (dialogue.dialogueContentSet[curContentIndex].storyLines.Count == 0)
            ShowChoiceDialogue(dialogue.dialogueContentSet[curContentIndex].choiceLines);
        else
        {
            curContentIndex = _curContentIndex;
            curStoryLineIndex = 0;
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
        isWaitingInput = false;
        if (curStoryLineIndex >= dialogue.dialogueContentSet[curContentIndex].storyLines.Count)
        {
            ShowChoiceDialogue(dialogue.dialogueContentSet[curContentIndex].choiceLines);
            return;
        }

        string text  = SharedMgr.InteractionMgr.GetDialogueReader.ReadText(dialogue.dialogueContentSet[curContentIndex].storyLines[curStoryLineIndex], out haveEvent);
        if (haveEvent)
        {
            curStoryLineIndex += 1;
            ShowDialogue();
        }
        else
            StartCoroutine(CTypeDialogueText(text));
    }

    public void ShowChoiceDialogue(List<Choice> _choiceLines)
    {
        if(_choiceLines == null || _choiceLines.Count == 0)
        {
            SharedMgr.InteractionMgr.EndConversation();
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
            if (_textCnt - i <= 1)
                isTypeText = false;
            text+= _text[i];
            dialogueTexts[1].text = text;
            yield return typeSecond;
        }
        isTypeText = false;
        isWaitingInput = true;
        curStoryLineIndex += 1;
        if (isAuto)
        {
            yield return delayAutoDialogueSecond;
            ShowDialogue();
        }
    }

    public void InputNext()
    {
        if (isWaitingInput)
            ShowDialogue() ;
        else
            SkipText();
    }

    public void SkipText()
    {
        if (isTypeText)
            StopAllCoroutines();
        else
            return;
        dialogueTexts[1].text = currentText;
        isTypeText = false;
        isWaitingInput = true;
        curStoryLineIndex += 1;
    }
    #endregion

    #region Maintain Conversation
    public void ContinueConversation(int _nextDialogueIndex)
    {
        CloseAllChoiceSlots();
        curContentIndex = _nextDialogueIndex;
        isChoiceActive = false;
        SetConversation(curContentIndex);
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
        autoButton.gameObject.SetActive(false);
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

    /******************************************/
    /**************  Interface  ***************/
    /******************************************/

    #region Interface
    bool isActive = true;
    public bool IsActive()
    {
        return isActive;
    }

    public void TurnOn()
    {
        isActive = true;
        dialogueFrame.SetActive(true);
    }

    public void TurnOff()
    {
        isActive = false;
        dialogueFrame.SetActive(false);
    }

    public void SetImages()
    {
        textDivisionImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Division_Top_Bar");
        autoButton.SetImage();
        int cnt = choiceSlots.Length;
        for(int i=0; i<cnt; i++)
        {
            choiceSlots[i].SetImage();
        }
    }
    #endregion
}
