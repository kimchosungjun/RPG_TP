using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace sapra.InfiniteLands
{
    public class TextureResult{
        private static readonly int 
            textureMasksID = Shader.PropertyToID("_splatMap"),
            textureMasksCountID = Shader.PropertyToID("_splatMapCount"),
            offsetID = Shader.PropertyToID("_MeshOffset"),
            resolutionID = Shader.PropertyToID("_Resolution"),
            mainTexID = Shader.PropertyToID("_MainTex"),
            terrainHeightNormalID = Shader.PropertyToID("_TerrainHeightNormal"),
            subTextureIndexID = Shader.PropertyToID("_SubTextureIndex"),
            densityMapID = Shader.PropertyToID("_DensityMap"),
            meshScaleID = Shader.PropertyToID("_MeshScale");
        
        public ExportedMultiResult NormalAndHeightTexture;

        public List<ExportedMultiResult> packs;
        public JobHandle handle;

        public List<Material> materials = new List<Material>();
        public Texture2DArray TextureMasksArray;
        public Texture2D NormalAndHeight;

        public MeshSettings settings;
        public TerrainConfig terrainConfig;

        public struct PreCalculated{
            public RenderTargetIdentifier texture;
            public int subIndex;
        }
        public Material groundMaterial;
        private Dictionary<IAsset, PreCalculated> calculated = new ();
        public TextureResult(List<ExportedMultiResult> exportedAssets){
            packs = exportedAssets;
        }
        public void CompleteRequest(ref TextureArrayPool texturePool, Material material, Action<Texture2DArray> DestroyTextures){
            handle.Complete();
            groundMaterial = material;

            Texture2D[] texturingMasks = packs
                .Where(a => typeof(IHoldTextures)
                .IsAssignableFrom(a.assetData.assetType))
                .SelectMany(a => a.textures.Select(t => t.ApplyTexture())).ToArray();

            if(texturePool == null && texturingMasks.Length > 0)
                texturePool = new TextureArrayPool(texturingMasks[0], texturingMasks.Length, false, DestroyTextures);
            
            if(texturePool != null){
                Texture2DArray textureMasksArray = texturePool.GetConfiguredArray("Masks", texturingMasks);
                if(textureMasksArray != null)
                    textureMasksArray.wrapMode = TextureWrapMode.Clamp;
                TextureMasksArray = textureMasksArray;
            }
            NormalAndHeight = NormalAndHeightTexture.textures[0].ApplyTexture();
        }

        RenderTargetIdentifier GetTextureOf(IAsset asset, out int subIndex){ 
            if(!calculated.TryGetValue(asset, out var result)){
                int index = packs
                    .Where(a => a.assetData.assets.Contains(asset)).SelectMany(a => a.assetData.assets)
                    .Select((b, index) => new{item = b, value = index}).Where(r => r.item.Equals(asset)).First().value;

                Texture2D vegMask = packs.Where(a => a.assetData.assets.Contains(asset)).SelectMany(a => a.textures).ToArray()[index/4].ApplyTexture();
                int sub = index - Mathf.FloorToInt(index / 4) * 4;    
                result = new PreCalculated(){
                    texture = vegMask,
                    subIndex = sub,
                };
                calculated.Add(asset, result);
            }       
            subIndex = result.subIndex;
            return result.texture;
        }
        

        public void DynamicMeshResultApply(CommandBuffer bf, ComputeShader compute, int kernelIndex, IAsset asset){
            RenderTargetIdentifier texture = GetTextureOf(asset, out int subIndex);
            bf.SetComputeTextureParam(compute, kernelIndex, textureMasksID, TextureMasksArray);
            bf.SetComputeTextureParam(compute, kernelIndex, terrainHeightNormalID, NormalAndHeight);
            bf.SetComputeTextureParam(compute, kernelIndex, densityMapID, texture);
            
            bf.SetComputeIntParam(compute, subTextureIndexID, subIndex); 
            if(TextureMasksArray != null)
                bf.SetComputeIntParam(compute, textureMasksCountID, TextureMasksArray.depth); 
            bf.SetComputeFloatParam(compute, meshScaleID, settings.MeshScale);
            bf.SetComputeIntParam(compute, resolutionID,  settings.TextureResolution);
            bf.SetComputeVectorParam(compute, offsetID, terrainConfig.Position);
        }
        public void ReloadMaterial(Material material, IAsset asset, bool procedural){
            material.SetInt(resolutionID, settings.TextureResolution);
            material.SetFloat(meshScaleID, settings.MeshScale);

            if (TextureMasksArray != null)
            {
                material.SetTexture(textureMasksID, TextureMasksArray);
                material.SetInt(textureMasksCountID, TextureMasksArray.depth);
            }
            
            if(procedural){
                material.EnableKeyword("_PROCEDURALTEXTURING");
            }else{
                Texture2D desired = GetDesiredTexture(asset, out Vector4 mask);
                material.SetTexture(mainTexID, desired);
                material.SetVector("_TextureMask", mask);
                material.DisableKeyword("_PROCEDURALTEXTURING");
            }
        }

        public Texture2D GetDesiredTexture(IAsset asset, out Vector4 mask){
            if(asset != null && packs != null){
                bool TheresOne = packs.Any(a => a.assetData.assets != null && a.assetData.assets.Contains(asset));
                if(TheresOne){
                    ExportedMultiResult pack = packs.FirstOrDefault(a => a.assetData.assets.Contains(asset));
                    int assetIndex = pack.assetData.assets.Select((text, index) => new {text, index}).First(a => a.text.Equals(asset)).index;

                    int actualTextureIndex = Mathf.FloorToInt(assetIndex/4.0f);
                    int maskIndex = assetIndex % 4;
                    BurstTexture selected = pack.textures[actualTextureIndex];
                    mask = new Vector4();
                    if(maskIndex >= 0 && maskIndex < 4)
                        mask[maskIndex] = 1;
                    return selected.ApplyTexture();
                }
                else
                    Debug.LogWarningFormat("Couldn't export {0}", asset.name);
            }
            mask = new Vector4(1,1,1,0);
            return NormalAndHeightTexture.textures[0].ApplyTexture();
            
        }

        public void AddMaterial(Material material){
            if(!materials.Contains(material))
                materials.Add(material);
        }
    }
}