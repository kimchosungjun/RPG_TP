using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWarnWindow : MonoBehaviour
{
    float showTime = 0f;
    bool isShowWarnText = false;

    [SerializeField] Animator warnAnim;
    [SerializeField] Text warnText;
    [SerializeField] Image windowImage;

    public void InActive()
    {
        this.gameObject.SetActive(false);
    }

    public void SetImage()
    {
        windowImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Window_Atlas", "Change_Warn");
    }

    public void ShowWarnText(UIEnums.CHANGE _changeType)
    {
        switch (_changeType)
        {
            case UIEnums.CHANGE.COOLDOWN:
                warnText.text = "쿨 다운 상태입니다.";
                break;
            case UIEnums.CHANGE.DEATH:
                warnText.text = "전투 불능 상태입니다.";
                break;
            case UIEnums.CHANGE.CANNOTCHANGE:
                warnText.text = "행동중에는 교체가 불가능합니다.";
                break;
        }

        showTime = 0f;
        if (isShowWarnText == false)
        {
            if (warnAnim.gameObject.activeSelf)
                warnAnim.Play("ChangeWarn_Active");

            isShowWarnText = true;
            StartCoroutine(CShowWarnText());
        }
    }

    IEnumerator CShowWarnText()
    {
        warnText.transform.parent.gameObject.SetActive(true);
        while (true)
        {
            if (showTime >= 2f)
            {
                warnAnim.Play("ChangeWarn_InActive");
                isShowWarnText = false;
                break;
            }
            showTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
