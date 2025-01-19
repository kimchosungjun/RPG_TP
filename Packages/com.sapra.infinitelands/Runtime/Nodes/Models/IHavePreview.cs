using System;

namespace sapra.InfiniteLands{
    // Creates preview in the editor
    public interface IHavePreview 
    {
        public Action<bool> OnImageUpdated{get;set;}
        public object GetTemporalTexture();

        public void GenerateTexture(GenerationSettings settings, IBurstTexturePool TexturePool);
        public void TogglePreview(bool value, bool forcedUpdate = false);
        public bool ShowPreview();
        public bool PreviewButtonEnabled();
    }
}