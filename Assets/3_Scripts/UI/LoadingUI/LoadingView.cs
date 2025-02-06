using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingView : MonoBehaviour
{
    [SerializeField] GameObject loadingView;
    [SerializeField] Text percentText;
    [SerializeField] Slider percentSlider;
    [SerializeField, Range(0f, 0.1f)] float fillAllTimer = 0.1f;
    [SerializeField, Tooltip("0:Frame,1:Fill")] Image[] gaugeImages;
    public void Init()
    {
        if (loadingView == null)
        {
            Transform[] childrenTfs = GetComponentsInChildren<Transform>();
            loadingView = childrenTfs[1].gameObject;
        }

        SetImage();

        if (loadingView != null)
            loadingView.gameObject.SetActive(true);

        if (percentSlider == null)
            percentSlider = GetComponentInChildren<Slider>(); 

        if (percentSlider != null)
        {
            percentSlider.maxValue = 100;
            percentSlider.value = 0;
        }

        if (percentText == null)
            percentText = GetComponentInChildren<Text>();

        if (percentText != null)
            percentText.text = "";
    }

    public void UpdateLoadingPercent(float _percent)
    {
        float percent = (_percent*100);
        percentText.text = (int)percent + "%";
        percentSlider.value = percent;
    }

    public void SetImage()
    {
        gaugeImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Loading_Bar");
        gaugeImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Loading_Line");
    }

    public void UpdateAllPercent() { StartCoroutine(CUpdateAllPercent());}

    IEnumerator CUpdateAllPercent()
    {
        float time = 0f;
        float startPoint = 90f;
        float endPoint = 100f;
        float fillValue = 0;
        while (time < fillAllTimer)
        {
            time += Time.fixedDeltaTime;
            fillValue = Mathf.Lerp(startPoint, endPoint, time / fillAllTimer);
            percentSlider.value = fillValue;
            percentText.text= (int)fillValue + "%";
            yield return new WaitForFixedUpdate();
        }
        percentSlider.value = 100;
        percentText.text = "100%";
        SharedMgr.UIMgr.LoadingUICtrl.FrameFadeIn();
    }
}
