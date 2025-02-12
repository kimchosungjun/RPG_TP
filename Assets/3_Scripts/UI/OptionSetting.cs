using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OptionSetting : MonoBehaviour
{
    [SerializeField, Tooltip("0:Low, 1:Mid, 2:High")] 
    List<RenderPipelineAsset> renderPipeLineSet;
    
    private void Start()
    {
        SetOptions();
    }

    public void SetOptions()
    {
        int graphicQuality = SharedMgr.SaveMgr.Option.graphicQuality;
        QualitySettings.SetQualityLevel(graphicQuality);
        QualitySettings.renderPipeline = renderPipeLineSet[graphicQuality];
        SharedMgr.SoundMgr.Setup();
    }
}
