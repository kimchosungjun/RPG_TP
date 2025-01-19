/* using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    public class AssetSelectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AssetSelectorView, GraphView.UxmlTraits>
        {
        }

        public AssetSelectorView()
        {
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Packages/com.sapra.infinitelands/Editor/UIBuilder/InfiniteLandsGraphEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public void PopulateView()
        {
            VisualElement target = contentContainer.Q("assets-content");
            target.Clear();
            AddTextures(target);
            AddVegetations(target);
        }

        void AddVegetations(VisualElement element)
        {
            string[] vegetationSetGUID = AssetDatabase.FindAssets("t:VegetationSet");

            foreach (string guids in vegetationSetGUID)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids);
                VegetationAsset set = AssetDatabase.LoadAssetAtPath<VegetationAsset>(path);
                element.Add(new Label(set.name));
            }
        }

        void AddTextures(VisualElement element)
        {
            string[] textureAssetGuid = AssetDatabase.FindAssets("t:TextureAsset");

            foreach (string guids in textureAssetGuid)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids);
                TextureAsset set = AssetDatabase.LoadAssetAtPath<TextureAsset>(path);
                element.Add(new Label(set.name));
            }
        }
    }
} */