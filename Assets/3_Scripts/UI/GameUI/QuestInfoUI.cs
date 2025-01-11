using UnityEngine;
using UnityEngine.UI;

public class QuestInfoUI : MonoBehaviour
{
    [SerializeField] QuestAwardSlot[] awardSlots;
    [SerializeField, Tooltip("0:Name, 1:Description, 2:Condition")] Text[] questTexts;
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
        questTexts[2].text = string.Empty;
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
