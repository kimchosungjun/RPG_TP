using System;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace sapra.InfiniteLands.UnityTerrain{
    [ExecuteAlways]
    public class UnityTerrainPainter : ChunkProcessor<TerrainWithHeightResult>, IGenerate<TerrainWithTextures>
    {
        public Material terrainMaterial;
        private ObjectPool<Material> _materialPool;

        public Action<TerrainWithTextures> onProcessDone { get; set; }
        public Action<TerrainWithTextures> onProcessRemoved { get; set; }
        public Action<IGraph, MeshSettings> onReload { get; set; }

        public IGraph graph{get; private set;}
        public MeshSettings settings{get; private set;}

        public int TextureResolution = 257;

        public override void Dispose()
        {
            if(_materialPool != null)
                _materialPool.Dispose();
        }

        private Material CreateMaterial(){
            return new(terrainMaterial);
        }
        public override void Initialize(IGraph generator, MeshSettings settings)
        {
            graph = generator;
            this.settings = settings;
            
            if(terrainMaterial == null){
                Debug.LogWarningFormat("No material has been set in {0}. Creating a temporal one", nameof(terrainMaterial));
                terrainMaterial = new Material(Shader.Find("Nature/Terrain/Diffuse"));
            }
            if(_materialPool == null)
                _materialPool = new UnityEngine.Pool.ObjectPool<Material>(CreateMaterial, actionOnDestroy: AdaptiveDestroy);
        }

        protected override void OnProcessAdded(TerrainWithHeightResult chunkWithHeight)
        {
            var chunk = chunkWithHeight.data;
            var newTerrain = chunkWithHeight.terrainData;
            chunk.assetsData.AddProcessor(this);
            var textures = chunk.assetsData.assets
                .Where(a => typeof(IHoldTextures).IsAssignableFrom(a.assetType));

            int count = 0;
            newTerrain.alphamapResolution = TextureResolution;

            TerrainWithTextures result = new TerrainWithTextures(){
                ID = chunk.terrainConfig.ID,
                data = chunk,
                terrainData = newTerrain,
                groundMaterial = _materialPool.Get()
            };
            if(textures.Any()){
                var onlyFirst = textures.First();

                TerrainLayer[] layers = new TerrainLayer[onlyFirst.assets.Count()];
                float[,,] details = new float[TextureResolution,TextureResolution, onlyFirst.assets.Count()];
                foreach(var asset in onlyFirst.assets){
                    var actual = asset as IHoldTextures;
                    TerrainLayer terrainLayer = new TerrainLayer();
                    for(int x = 0; x < TextureResolution; x++){
                        for(int y = 0; y < TextureResolution; y++){
                            float height = Remap(chunk.meshSettings.Resolution+1, TextureResolution, y, x, onlyFirst.map, 
                                (chunk.meshSettings.Resolution+1)*(chunk.meshSettings.Resolution+1)*count, onlyFirst.map.Length-1);

                            details[x,y, count] = height;
                        }
                    }
                    terrainLayer.diffuseTexture = actual.GetTextures()[0].GetTexture2D();
                    terrainLayer.normalMapTexture = actual.GetTextures()[2].GetTexture2D();
                    terrainLayer.tileSize = Vector2.one*actual.GetSettings().TextureSize;
                    terrainLayer.name = actual.name;
                    layers[count] = terrainLayer;
                    count++;
                }
                for(int x = 0; x < TextureResolution; x++){
                    for(int y = 0; y < TextureResolution; y++){
                        float total = 0;
                        for(int c = 0; c < count; c++){
                            total += details[x,y, c];
                        }
                        for(int c = 0; c < count; c++){
                            details[x,y, c] /= total;
                        }
                        if(total <= 0.1f)
                            details[x,y, 0] = 1;
                    }
                }
                newTerrain.terrainLayers = layers;
                newTerrain.SetAlphamaps(0,0, details);
                chunk.assetsData.RemoveProcessor(this);
            }
            onProcessDone?.Invoke(result);
        }

        private float Remap(int currentResolution, int targetResolution, int xTarget, int yTarget, NativeArray<float> map, int offset, int maxLength){
            float factor = (float)currentResolution/(float)targetResolution;
            float ogX = xTarget*factor;
            float ogY = yTarget*factor;

            if(xTarget == 0)
                ogX = 0;
            else if(xTarget == targetResolution-1)
                ogX = currentResolution-1;
            if(yTarget == 0)
                ogY = 0;
            else if(yTarget == targetResolution-1)
                ogY = currentResolution-1;

            int btmX = Mathf.Min(Mathf.FloorToInt(ogX), currentResolution);
            int btmY = Mathf.Min(Mathf.FloorToInt(ogY), currentResolution);
            int topX = Mathf.Min(Mathf.CeilToInt(ogX), currentResolution);
            int topY = Mathf.Min(Mathf.CeilToInt(ogY), currentResolution);
            
            int maxSingleLength = MapTools.LengthFromResolution(currentResolution-1)-1;
            int indBtmX = Mathf.Min(maxSingleLength, btmX+btmY*(currentResolution));
            int indBtmY = Mathf.Min(maxSingleLength, btmX+topY*(currentResolution));
            int indTopX = Mathf.Min(maxSingleLength, topX+btmY*(currentResolution));
            int indTopY = Mathf.Min(maxSingleLength, topX+topY*(currentResolution));


            int leftDownIndex =  Mathf.Min(indBtmX+offset,maxLength);
            int leftUpIndex =  Mathf.Min(indBtmY+offset,maxLength);
            int rightDownIndex =  Mathf.Min(indTopX+offset,maxLength);
            int righTUpIndex =  Mathf.Min(indTopY+offset,maxLength);

            float leftDown = map[leftDownIndex];
            float leftUp = map[leftUpIndex];
            float rightDown = map[rightDownIndex];
            float righTUp = map[righTUpIndex];

            float fcX = ogX-btmX;
            float fcy = ogY-btmY;
            float interBotm = Mathf.Lerp(leftDown, rightDown, fcX);
            float interUp = Mathf.Lerp(leftUp, righTUp, fcX);
            return Mathf.Lerp(interBotm, interUp, fcy);
        }

        protected override void OnProcessRemoved(TerrainWithHeightResult chunk)
        {
            
        }
    }
}