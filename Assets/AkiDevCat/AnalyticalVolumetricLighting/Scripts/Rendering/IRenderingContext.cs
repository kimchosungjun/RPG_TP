using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AkiDevCat.AVL.Rendering
{
    public interface IRenderingContext
    {
        AVLFeatureSettings Settings { get; }
        
        RenderTargetIdentifier CameraColorTarget { get; }
        
        RenderTargetIdentifier CameraDepthTarget { get; }
        
        GraphicsBuffer GlobalLightBuffer { get; set; }
        
        uint GlobalLightBufferSize { get; set; }
        
        uint GlobalMaskBufferSize { get; set; }
        
        GraphicsBuffer LightIndexBuffer { get; set; }
        
        GraphicsBuffer LightClusterBuffer { get; set; }
        
        GraphicsBuffer GlobalMaskBuffer { get; set; }
        
        GraphicsBuffer MaskIndexBuffer { get; set; }
        
        GraphicsBuffer MaskInverseIndexBuffer { get; set; }

        List<LightData> GlobalLightList { get; set; }
        
        List<MaskData> GlobalMaskList { get; set; }
    }
}