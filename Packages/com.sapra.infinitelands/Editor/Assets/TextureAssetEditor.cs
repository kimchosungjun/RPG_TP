using UnityEngine;
using UnityEditor;

namespace sapra.InfiniteLands.Editor{
    [CustomEditor(typeof(TextureAsset), true)]
    [CanEditMultipleObjects]
    public class TextureAssetEditor : UnityEditor.Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {   
            TextureAsset example = (TextureAsset)target;
            if (example == null || example.Albedo == null)
                return null;

            Texture2D previewTexture = null;
            while (previewTexture == null)
            {
                previewTexture = AssetPreview.GetAssetPreview(example.Albedo);
            }
            Texture2D tex = new Texture2D (width, height);
            EditorUtility.CopySerialized(previewTexture, tex);

            return tex;
        }
    }
}

/* [InitializeOnLoad]
public class CustomThumbnailRenderer
{
    static CustomThumbnailRenderer()
    {
        // Make sure we unbind first since we could bind twice when scripts are reloaded
        EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUICallback;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUICallback;
    }
    static void ProjectWindowItemOnGUICallback(string guid, Rect selectionRect)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        // If we're a directory, ignore it.
        if (assetPath.LastIndexOf('.') == -1)
        {
            return;
        }

        // If we're not a textureAsset, ignore it
        TextureAsset go = AssetDatabase.LoadAssetAtPath<TextureAsset>(assetPath);
        if (go == null)
        {
            return;
        }


        Texture2D tex = go.Albedo;
        if (tex == null)
        {
            Debug.LogError("Failed to load thumb for " + assetPath);
        }
        Rect drawRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height - 15);
        EditorGUI.DrawPreviewTexture(drawRect, tex);
    }
} */