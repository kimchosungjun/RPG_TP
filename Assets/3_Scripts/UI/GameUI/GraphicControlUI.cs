using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GraphicControlUI : MonoBehaviour, ITurnOnOffUI
{
    [SerializeField] GameObject graphicUI;
    [SerializeField] List<RenderPipelineAsset> renderPipeLineSet;  
    [SerializeField] Image optionImage;
    [SerializeField] Dropdown dropDown;

    // Only Use for Indicate 
    enum Option { Low = 0, Mid = 1, High = 2 }

    public void Init()
    {
        SetImages();
        SetValues();
    }

    public void SetImages()
    {
        optionImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Dialogue_Bar");
    }

    public void SetValues()
    {
        if (dropDown.value != SharedMgr.SaveMgr.Option.graphicQuality)
            dropDown.value = SharedMgr.SaveMgr.Option.graphicQuality;

        dropDown.onValueChanged.RemoveAllListeners();
        dropDown.onValueChanged.AddListener(SetRenderPipeline);
    }

    public void SetRenderPipeline(int _pipeLine)
    {
        SharedMgr.SoundMgr.PressButtonSFX();
        QualitySettings.SetQualityLevel(_pipeLine);
        QualitySettings.renderPipeline = renderPipeLineSet[_pipeLine];
        SharedMgr.SaveMgr.Option.graphicQuality = _pipeLine;
    }

    public void TurnOff()
    {
        if (graphicUI.gameObject.activeSelf)
            graphicUI.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        if (graphicUI.gameObject.activeSelf == false)
            graphicUI.gameObject.SetActive(true);
    }
}
