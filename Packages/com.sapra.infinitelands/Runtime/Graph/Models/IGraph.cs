using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands{
    public interface IGraph
    {
        public string name{get; set;}
        public IEnumerable<InfiniteLandsNode> GetNodes();
        public IEnumerable<AssetWithType> GetAssets();

        public HeightOutputNode GetOutputNode();
        public Dictionary<IAsset, ILoadAsset[]> GetAssetLoaders();
        public Action OnValuesChanged{get; set;}

        public bool ValidOutput{get;}
        public bool _autoUpdate{get;}

        public Vector2 MinMaxHeight{get;}

        public void ValidationCheck();
        public void Initialize();

        #if UNITY_EDITOR
        public GroupBlock CreateGroup(string name, Vector2 position, List<string> elementsGUIDS);
        public StickyNoteBlock CreateStickyNote(Vector2 position);
        public InfiniteLandsNode CreateNode(Type type, Vector2 position);

        public StickyNoteBlock CreateStickyNoteFromJson(string JsonData, Vector2 position);
        public GroupBlock CreateGroupFromJson(string JsonData, Vector2 position);
        public InfiniteLandsNode CreateNodeFromJson(Type type, string JsonData, Vector2 position);

        public void AddElementsToGroup(GroupBlock group, IEnumerable<string> guids);
        public bool Connect(InfiniteLandsNode node, string inputPortName, InfiniteLandsNode provider, string providerPortName);
        #endif
    }
}