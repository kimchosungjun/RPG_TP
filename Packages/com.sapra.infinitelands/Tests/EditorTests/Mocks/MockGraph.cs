using System;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.InfiniteLands.Tests{
    public class MockGraph : IGraph
    {
        public string name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action OnValuesChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ValidOutput => throw new NotImplementedException();

        public bool _autoUpdate => throw new NotImplementedException();

        public Vector2 MinMaxHeight => throw new NotImplementedException();

        public void AddElementsToGroup(GroupBlock group, IEnumerable<string> guids)
        {
            throw new NotImplementedException();
        }

        public bool Connect(InfiniteLandsNode node, string inputPortName, InfiniteLandsNode provider, string providerPortName)
        {
            throw new NotImplementedException();
        }

        public GroupBlock CreateGroup(string name, Vector2 position, List<string> elementsGUIDS)
        {
            throw new NotImplementedException();
        }

        public GroupBlock CreateGroupFromJson(string JsonData, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public InfiniteLandsNode CreateNode(Type type, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public InfiniteLandsNode CreateNodeFromJson(Type type, string JsonData, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public StickyNoteBlock CreateStickyNote(Vector2 position)
        {
            throw new NotImplementedException();
        }

        public StickyNoteBlock CreateStickyNoteFromJson(string JsonData, Vector2 position)
        {
            throw new NotImplementedException();
        }

        public Dictionary<IAsset, ILoadAsset[]> GetAssetLoaders()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssetWithType> GetAssets()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<InfiniteLandsNode> GetNodes()
        {
            throw new NotImplementedException();
        }

        public HeightOutputNode GetOutputNode()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void ValidationCheck()
        {
            throw new NotImplementedException();
        }
    }
}