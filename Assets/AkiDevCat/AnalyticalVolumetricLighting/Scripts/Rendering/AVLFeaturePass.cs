using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AkiDevCat.AVL.Rendering
{
    public abstract class AVLFeaturePass : ScriptableRenderPass
    {
        protected IRenderingContext RenderingContext { get; }

        protected AVLFeaturePass(IRenderingContext renderingContext)
        {
            RenderingContext = renderingContext;
        }
        
        public abstract void Cleanup();
    }
}