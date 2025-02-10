using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoUI : MonoBehaviour
{
    [SerializeField] QuestAwardSlot[] awardSlots;
    [SerializeField, Tooltip("0:Name, 1:Description")] Text[] questTexts;
    [SerializeField] Text[] questConditionTexts;
    [SerializeField] GameObject infoParent;
    [SerializeField] Image nameFrameImage;

    public void Init()
    {
        SetImages();
    }

    public void Active(QuestSOData _data)
    {
        infoParent.SetActive(true);
        questTexts[0].text = _data.GetQuestName;
        questTexts[1].text = _data.GetQuestDescription;

        List<QuestConditionData> conditions = _data.GetQuestConditionSet;
        int cnt = conditions.Count;
        Debug.Log(cnt);
        for(int i=0; i<cnt; i++)
        {
            bool isMeet = false;
            questConditionTexts[i].text = conditions[i].GetQuestDescription(ref isMeet);
            Debug.Log(conditions[i].GetQuestDescription(ref isMeet));
            if(isMeet)
                questConditionTexts[i].color = Color.green;
            else
                questConditionTexts[i].color = Color.yellow;

            if (questConditionTexts[i].gameObject.activeSelf == false)
                questConditionTexts[i].gameObject.SetActive(true);
        }

        for(int i=cnt; i<3; i++)
        {
            if (questConditionTexts[i].gameObject.activeSelf)
                questConditionTexts[i].gameObject.SetActive(false);
        }   
    }

    public void InActive()
    {
        infoParent.SetActive(false);
        questTexts[0].text = string.Empty;
        questTexts[1].text = string.Empty;
        questTexts[2].text = string.Empty;
    }

    public void SetImages()
    {
        nameFrameImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Gradient_Line");
        int awardCnt = awardSlots.Length;
        for (int i = 0; i < awardCnt; i++)
        {
            awardSlots[i].Init();
        }
    }
}
