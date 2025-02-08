using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GraphicControlUI : MonoBehaviour, ITurnOnOffUI
{
    [SerializeField] GameObject graphicUI;
    [SerializeField] List<RenderPipelineAsset> renderPipeLineSet;  // 5_SoData -> Quality
    [SerializeField] Image optionImage;

    // Only Use for Indicate 
    enum Option { Low=0,Mid=1,High=2}   
    
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        optionImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Dialogue_Bar");
    }
    
    public void SetRenderPipeline(int _pipeLine)
    {
        QualitySettings.SetQualityLevel(_pipeLine);
        QualitySettings.renderPipeline = renderPipeLineSet[_pipeLine];
        ApplySetting();
    }

    public void TurnOff()
    {
        if(graphicUI.gameObject.activeSelf)
            graphicUI.gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        if (graphicUI.gameObject.activeSelf == false)
            graphicUI.gameObject.SetActive(true);
    }

    public void ApplySetting()
    {
        // To Do 
        // To Do Make Quality Mgr
    }
}
