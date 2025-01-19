using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace sapra.InfiniteLands
{
    public class GenerationSettings
    {
        public string BranchGUID;
        public MeshSettings meshSettings;
        public TerrainConfig terrain;
        public NativeArray<float> globalMap;
        public int pointsLength;
        public float ratio;

        public DataManager manager;
        public IndexManager indices;
        
        public IGeneratePoints PointMaker;
        public GenerationSettings derivedFrom;
        
        public NativeArray<float3> points;
        public JobHandle dependancy;

        protected GenerationSettings(int layers, float ratio, MeshSettings settings, TerrainConfig terrain, DataManager manager, IndexManager indices, string RequestGuid, 
            IGeneratePoints PointMaker)
        {
            this.meshSettings = settings;
            int vertexCount = MapTools.LengthFromResolution(settings.Resolution);
            this.ratio = ratio;
            this.terrain = terrain;
            this.pointsLength = vertexCount;
            this.manager = manager;
            this.indices = indices;

            this.globalMap = manager.GetReturnableArray<float>(terrain.ID, vertexCount*layers);
            this.BranchGUID = RequestGuid;
            this.PointMaker = PointMaker;          

            this.dependancy = PointMaker.GiveMePoints(this, out points, default, default);  
        }

        protected GenerationSettings(int layers, float ratio, MeshSettings settings, string RequestGuid, IGeneratePoints PointMaker, GenerationSettings derived)
        {
            this.meshSettings = settings;
            int vertexCount = MapTools.LengthFromResolution(settings.Resolution);
            this.ratio = ratio;
            this.terrain = derived.terrain;
            this.pointsLength = vertexCount;
            this.manager = derived.manager;
            this.indices = derived.indices;
            this.globalMap = derived.manager.GetReturnableArray<float>(terrain.ID, vertexCount * layers);
            this.BranchGUID = RequestGuid;
            this.derivedFrom = derived;
            this.PointMaker = PointMaker;      

            this.dependancy = PointMaker.GiveMePoints(this, out points, derived.points, derived.dependancy);  
        }
        
        public static GenerationSettings NewDerivedSettings(int layers, IGeneratePoints PointMaker, string guid, GenerationSettings derived){
            return new GenerationSettings(layers,derived.ratio, derived.meshSettings, guid, PointMaker, derived);
        }

        public static GenerationSettings NewSeedSettings(int layers, int seedOffset, GenerationSettings derived){
            MeshSettings previous = derived.meshSettings;
            previous.Seed += seedOffset;
            return new GenerationSettings(layers,derived.ratio, previous, GetSeedGuid(derived.BranchGUID, seedOffset), derived.PointMaker, derived);
        }

        public static string GetSeedGuid(string ogGuid, int seedOffset){
            return ogGuid+"/Seed:"+seedOffset;
        }

        public static string GetGUID(string ogGuid, int resolution, float meshscale){
            return ogGuid+"/Res:"+resolution+"/Scale:"+meshscale;
        }

        public static GenerationSettings NewSettings(int layers, float ratio, MeshSettings settings, TerrainConfig terrain, DataManager manager, IndexManager indices, IGeneratePoints pointMaker, string guid){
            return new GenerationSettings(layers,ratio, settings, terrain, manager,indices, guid, pointMaker);
        }
        public static GenerationSettings NewSettings(int layers, int EditorResolution, float MeshScale, int Seed, Vector3 WorldOffset,float ratio, DataManager manager,IndexManager indices,IGeneratePoints pointMaker, string guid)
        {
            TerrainConfig config = new TerrainConfig
            {
                Position = WorldOffset,
            };

            MeshSettings meshSettings = new MeshSettings
            {
                Resolution = EditorResolution,
                MeshScale = MeshScale,
                Seed = Seed,
                meshType = MeshSettings.MeshType.Normal
            };

            GenerationSettings settings = NewSettings(layers, ratio, meshSettings, config, manager,indices, pointMaker, guid);
            return settings;
        }

        public Matrix4x4 GetVectorMatrix(GenerationModeNode nodeMode){
            Vector3 up;
            switch(nodeMode){
                case GenerationModeNode.Default:
                    up = meshSettings.generationMode.Equals(MeshSettings.GenerationMode.RelativeToWorld) ? terrain.TerrainNormal : Vector3.up;
                    break;
                case GenerationModeNode.RelativeToWorld:
                    up = terrain.TerrainNormal;
                    break;
                default: 
                    up = Vector3.up;
                    break;
            }

            return Matrix4x4.TRS(Vector3.zero, Quaternion.FromToRotation(Vector3.up, up), Vector3.one).inverse;
        }
    }
}