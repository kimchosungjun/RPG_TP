using System;

namespace sapra.InfiniteLands{
    public interface IExportTextures{
        public string description{get;}
        public int GetTextureResolution();
        public void SetExporterResolution(int resolution);
        public ExportedMultiResult GenerateHeightTexture(WorldFinalData worldData);
        public ExportedMultiResult GenerateDensityTextures(AssetData data);
        
        public void ReturnExportedData(ExportedMultiResult data);
        public void DestroyTextures(Action<UnityEngine.Object> Destroy);
        public void ResetExporter();
    }
}
