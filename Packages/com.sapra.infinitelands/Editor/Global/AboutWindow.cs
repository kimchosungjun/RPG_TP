using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

namespace sapra.InfiniteLands.Editor{
    public class AboutWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Window/InfiniteLands/About")]
        public static void OpenPopup()
        {
            AboutWindow wnd = GetWindow<AboutWindow>();
            wnd.titleContent = new GUIContent("About Infinite Lands");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(rootVisualElement);

            Button gettingStarted = root.Q<Button>("getting-started");
            gettingStarted.clicked += OpenGettingStarted;

            Button documentation = root.Q<Button>("documentation");
            documentation.clicked += OpenDocumentation;

            Button discord = root.Q<Button>("discord");
            discord.clicked += OpenDiscord;

            Button review = root.Q<Button>("leave-review");
            review.clicked += OpenReview;

            Button samples = root.Q<Button>("import-samples");
            samples.clicked += OpenPackage;

            StreamReader reader = new StreamReader("Packages/com.sapra.infinitelands/CHANGELOG.md");
            Label changeLog =  root.Q<Label>("changelog");
            changeLog.text = reader.ReadToEnd();
            reader.Close();
        }
        private void OpenGettingStarted(){
            Application.OpenURL("https://ensapra.com/packages/infinite_lands/getting-started");
        }
        private void OpenDocumentation(){
            Application.OpenURL("https://ensapra.com/packages/infinite_lands");
        }
        private void OpenDiscord(){
            Application.OpenURL("https://discord.gg/xkkWP6H6eJ");
        }
        private void OpenReview(){
            Application.OpenURL("https://assetstore.unity.com/packages/slug/276010#reviews");
        }
        private void OpenPackage(){
            UnityEditor.PackageManager.UI.Window.Open("com.sapra.infinitelands");
        }
    }
}