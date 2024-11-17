using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public partial class ResourceMgr
{
    [NonReorderable] 
    Dictionary<string, SpriteAtlas> spriteAtlasGroup = new Dictionary<string, SpriteAtlas>();

    public Sprite GetSpriteAtlas(string _Atlas, string _spriteName)
    {
        if (spriteAtlasGroup.ContainsKey(_Atlas))
            return spriteAtlasGroup[_Atlas].GetSprite(_spriteName);

        Object loadAtlas = Resources.Load("Atlas/" + _Atlas);
        if (loadAtlas == null) return null;

        SpriteAtlas spriteAtlas = loadAtlas as SpriteAtlas;
        if (spriteAtlas != null)
        {
            spriteAtlasGroup.Add(_Atlas, spriteAtlas);
            return spriteAtlas.GetSprite(_spriteName);
        }

        return null;
    }

    public void RemoveAtlas(string _atlasName)
    {
        if (spriteAtlasGroup.ContainsKey(_atlasName))
            spriteAtlasGroup.Remove(_atlasName); 
    }

    public void ClearAtlas()
    {
        if (spriteAtlasGroup.Count != 0)
            spriteAtlasGroup.Clear();
    }
}
