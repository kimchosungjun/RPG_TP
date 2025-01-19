using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace sapra.InfiniteLands.Editor{
    public class InfiniteLandsGraphEditor : EditorWindow, IHasCustomMenu
    {
        [SerializeField] TerrainGenerator treeInstance;
        TerrainGeneratorView treeView;
        IBurstTexturePool texturePool;

        [SerializeField] int EditorResolution = 100;
        GraphSettings settings{
            get{
                if(internalSettings == null)
                    internalSettings = CreateInstance<GraphSettings>();
                return internalSettings;
            }
        }
        [SerializeField] GraphSettings internalSettings;

        [SerializeField] bool SyncSettings = false;

        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;

        ToolbarButton RefreshButton;
        ToolbarButton SaveButton;
        ToolbarButton ExportButton;

        Toggle AutoUpdateToggle;

        Vector2Field WorldOffsetField;
        IntegerField SeedField;
        IntegerField ResolutionField;
        FloatField ScaleField;

        Toggle SyncSettingsToggle;

        #region Window Options
        private void OnEnable() {
            Undo.undoRedoPerformed -= OnUndoRedo;
            AssemblyReloadEvents.beforeAssemblyReload -= SaveTree;
            
            GrabUIElements();
        
            AssemblyReloadEvents.beforeAssemblyReload += SaveTree;
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        private void OnDisable(){
            Undo.undoRedoPerformed -= OnUndoRedo;
            AssemblyReloadEvents.beforeAssemblyReload -= SaveTree;
        }
        private static InfiniteLandsGraphEditor GenerateWindow(TerrainGenerator asset){
            bool ThereAreInstances = HasOpenInstances<InfiniteLandsGraphEditor>();

            if (ThereAreInstances){
                InfiniteLandsGraphEditor opened = FocusWindowIfOpened(asset);
                if(opened != null)
                    return opened;
            }

            var requiredAttribute = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
            InfiniteLandsGraphEditor wnd = CreateWindow<InfiniteLandsGraphEditor>(requiredAttribute);
            return wnd;
        }

        private static InfiniteLandsGraphEditor FocusWindowIfOpened(TerrainGenerator asset){
            InfiniteLandsGraphEditor window = FindWindow(asset);
            if(window != null)
                window.Focus();
            return window;
        }

        public static InfiniteLandsGraphEditor FindWindow(TerrainGenerator asset){
            InfiniteLandsGraphEditor[] allWindows = Resources.FindObjectsOfTypeAll<InfiniteLandsGraphEditor>();
            foreach(InfiniteLandsGraphEditor window in allWindows){
                if(!window)
                    continue;
                if(window.IsTheSameTree(asset))
                    return window;
            }
            return null;
        }

        public static void ReloadWindow(TerrainGenerator asset){
            InfiniteLandsGraphEditor window = FindWindow(asset);
            if(window == null)
                return;
            window.LoadNewAsset(asset);
        }

        [OnOpenAsset]
        public static bool OpenGraphAsset(int instanceID)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (!(asset is TerrainGenerator generator)) return false;
            else{
                return OpenGraphAsset(generator);
            }
        }

        public static bool OpenGraphAsset(TerrainGenerator asset)
        {
            var wnd = GenerateWindow(asset);
            wnd.LoadNewAsset(asset);
            return true;
        }
        
        private static void CloseAllWindows(){
            InfiniteLandsGraphEditor[] allWindows = Resources.FindObjectsOfTypeAll<InfiniteLandsGraphEditor>();
            foreach(InfiniteLandsGraphEditor window in allWindows){
                window.Close();
            }
        }
        private void FocusAsset(){
            if(treeInstance != null){
                EditorGUIUtility.PingObject(treeInstance);
            }
        }

        private void OnDestroy() {
            SaveTree();
        }
        public static void CloseWindow(TerrainGenerator asset){
            InfiniteLandsGraphEditor window = FocusWindowIfOpened(asset);
            window?.Close();
        }
        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Close all Tabs"), false, CloseAllWindows);
            menu.AddItem(new GUIContent("Find reference in Project"), false, FocusAsset);
        }
        #endregion

        public bool IsTheSameTree(IGraph target){
            return treeInstance != null && target != null && treeInstance.Equals(target);
        }


        #region UI
        public void CreateGUI()
        {
            if(treeInstance != null)
                LoadAsset();
        }


        private void GrabUIElements(){
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(rootVisualElement);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Packages/com.sapra.infinitelands/Editor/UIBuilder/InfiniteLandsGraphEditor.uss");
            root.styleSheets.Add(styleSheet);
            treeView = root.Q<TerrainGeneratorView>();
            treeView.focusable = true;
            treeView.RegenerateNodeSystem -= RegenerateNodeSystem;
            treeView.RegenerateNodeSystem += RegenerateNodeSystem;
            
            RefreshButton = root.Q<ToolbarButton>("regenerate");
            SaveButton = root.Q<ToolbarButton>("save");
            ExportButton = root.Q<ToolbarButton>("export");


            ScaleField = root.Q<FloatField>("editor-scale");
            WorldOffsetField = root.Q<Vector2Field>("editor-offset");
            SeedField = root.Q<IntegerField>("editor-seed");
            SeedField = root.Q<IntegerField>("editor-seed");
            SyncSettingsToggle = root.Q<Toggle>("editor-sync");
            AutoUpdateToggle = root.Q<Toggle>("auto-regenerate");
            ResolutionField = root.Q<IntegerField>("editor-resolution");
        }

        private void UpdateAllUI(){
            SettingsWindow();
            ToolbarButtons();
            treeView.Focus();
        }

        void SettingsWindow(){
            ScaleField.bindingPath = nameof(settings.MeshScale);
            ScaleField.isDelayed = true;
            ScaleField.RegisterValueChangedCallback(a =>
            {
                float value = Mathf.Max(100, a.newValue);
                ScaleField.SetValueWithoutNotify(value);
                UpdateTextures();
            });

            ResolutionField.value = EditorResolution;
            ResolutionField.isDelayed = true;
            ResolutionField.RegisterValueChangedCallback(a =>
            {
                int resolution = Mathf.Clamp(a.newValue, 10, 200);
                EditorResolution = resolution;
                ResolutionField.SetValueWithoutNotify(resolution);
                UpdateTextures();
            });
            
            WorldOffsetField.bindingPath = nameof(settings.WorldOffset);
            WorldOffsetField.RegisterValueChangedCallback(a => UpdateTextures());

            SeedField.bindingPath = nameof(settings.Seed);
            SeedField.RegisterValueChangedCallback(a => UpdateTextures());

            SyncSettingsToggle.value = SyncSettings;
            SyncSettingsToggle.RegisterValueChangedCallback(a =>
            {
                SyncSettings = a.newValue;
                UpdateUI(SyncSettings);
            });

            AutoUpdateToggle.SetEnabled(treeInstance != null);
            if (AutoUpdateToggle != null)
            {
                AutoUpdateToggle.bindingPath = nameof(treeInstance.AutoUpdate);
                AutoUpdateToggle.SetEnabled(treeInstance);

                if(treeInstance!= null)
                    AutoUpdateToggle.Bind(new SerializedObject(treeInstance));
            }

            UpdateUI(SyncSettings);
        }
        void ToolbarButtons(){
            RefreshButton.SetEnabled(treeInstance != null);
            if (RefreshButton != null)
            {
                RefreshButton.clicked -= Regenerate;
                RefreshButton.clicked += Regenerate;
            }

            SaveButton.SetEnabled(treeInstance != null);
            if (SaveButton != null)
            {
                SaveButton.clicked -= SaveTree;
                SaveButton.clicked += SaveTree;
            }

            ExportButton.SetEnabled(treeInstance != null);
            if (ExportButton != null)
            {
                ExportButton.clicked -= ExportTree;
                ExportButton.clicked += ExportTree;
            }
        }
        #endregion

        void SaveTree()
        {   
            if(treeInstance == null)
                return;

            EditorUtility.SetDirty(treeInstance);
            AssetDatabase.SaveAssetIfDirty(treeInstance);
            RegenerateNodeSystem();
        }
        

        void ExportTree()
        {
            ExportPopup.OpenPopup(treeInstance, settings);
        }

        public void ForceRefresh()
        {
            if (treeInstance != null)
                treeInstance.OnValuesChanged?.Invoke();
        }

        public void RegenerateNodeSystem()
        {
            if (treeInstance != null){
                if(treeInstance.AutoUpdate)
                    ForceRefresh();
                UpdateTextures();
            }
        }

        public void UpdateTextures(){
            if(treeInstance != null){
                EditorGenerator generator = new EditorGenerator(treeInstance);
                if(texturePool == null || texturePool.GetTextureResolution() != EditorResolution){
                    texturePool?.DestroyBurstTextures(DestroyImmediate);
                    texturePool = new BurstTexturePool(EditorResolution);
                }
                generator.GenerateEditorVisuals(GetInstanceID().ToString(), EditorResolution, settings.MeshScale, settings.Seed, settings.WorldOffset, texturePool);
            }
        }

        private void OnUndoRedo()
        {
            if(!AnythingDirty())
                return;
                
            if(treeInstance != null)
                treeView.Initialize(treeInstance, this);
            SaveTree();
        }

        private bool AnythingDirty(){
            bool drtyAsset = EditorUtility.IsDirty(treeInstance);
            if(drtyAsset)
                return true;
            foreach(InfiniteLandsNode node in treeInstance.nodes){
                if(EditorUtility.IsDirty(node))
                    return true;
            }
            return false;
        }   
        private void Regenerate(){
            bool generated = false;
            var visualizers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IVisualizeTerrain>();
            foreach(IVisualizeTerrain visualizer in visualizers){
                if(IsTheSameTree(visualizer.graph)){
                    visualizer.ForceGeneration(true);
                    generated = true;
                }
            }
            if(!generated){
                Debug.LogWarning("There are no generators with this graph. Nothing has been generated");
            }
        }
        private void UpdateUI(bool state)
        {
            SeedField.SetEnabled(!state);
            SetEnabledVector2(WorldOffsetField, !state);
            ScaleField.SetEnabled(!state);

            SerializedObject serializedSettings;
            if (state)
            {
                GraphSettings globalSettings = GraphSettingsController.GetSettings();
                serializedSettings = new SerializedObject(globalSettings);
            }
            else
                serializedSettings = new SerializedObject(settings);
            
            WorldOffsetField.Bind(serializedSettings);
            SeedField.Bind(serializedSettings);
            ScaleField.Bind(serializedSettings);
        }

        private void SetEnabledVector2(Vector2Field field, bool value)
        {
            Label lbl = field.Q<Label>();
            FloatField vlx = field.Q<FloatField>("unity-x-input");
            FloatField vly = field.Q<FloatField>("unity-y-input");

            lbl.SetEnabled(value);
            vlx.SetEnabled(value);
            vly.SetEnabled(value);
        }
        private void ChangeAssetState(bool value){
            Label assets = this.rootVisualElement.Q<Label>("no-asset");
            assets.visible = value;
        }

        private void LoadNewAsset(TerrainGenerator tree){
            if(IsTheSameTree(tree))
                return;
            if(tree != null && !AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                return;

            treeInstance = tree;
            LoadAsset();
        }

        private void LoadAsset(){
            treeInstance?.ValidateThatItHasOutput();
            treeView.Initialize(treeInstance, this);
            UpdateAllUI();
            ReloadTitle(treeInstance);
            ChangeAssetState(treeInstance == null);
            AutoUpdateToggle.SetEnabled(treeInstance != null);
        }
        private void ReloadTitle(TerrainGenerator tree){
            if(tree == null)
                titleContent = new GUIContent("None");
            else
                titleContent = new GUIContent(tree.name, GetIcon(tree));
        }
        private Texture GetIcon(TerrainGenerator tree){
            if(tree.GetType() == typeof(BiomeTree))
                return EditorGUIUtility.IconContent("d_AnimatorOverrideController Icon").image;
            else
                return EditorGUIUtility.IconContent("d_AnimatorController Icon").image;
        }
    }
}