using Unity.Jobs;

namespace sapra.InfiniteLands{
    public struct ExportedMultiResult{
        public JobHandle job{get; private set;}
        public BurstTexture[] textures{get; private set;}
        public AssetData assetData{get; private set;}
        public static ExportedMultiResult Default => new ExportedMultiResult(){
            job = default,
            textures = new BurstTexture[0],
        };

        public ExportedMultiResult(JobHandle job, BurstTexture[] textures, AssetData assets){
            this.job = job;
            this.textures = textures;
            this.assetData = assets;
        }
    }
}