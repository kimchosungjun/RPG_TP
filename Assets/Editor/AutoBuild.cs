using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AutoBuild
{
    static string[] SCENES = FindEnableEditorScenes();
    static string TARGET_DIR = "Build";
    const string APP_NAME = "AshenVanguard";
    //static int BUILD_DATE = 0;

    /// <summary>
    /// Get Scene Names
    /// </summary>
    /// <returns></returns>
    static string[] FindEnableEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        EditorBuildSettingsScene[] sceneSet = EditorBuildSettings.scenes;
        int sceneCnt = sceneSet.Length;
        for(int i=0; i<sceneCnt; i++)
        {
            if (!sceneSet[i].enabled)
                continue;
            EditorScenes.Add(sceneSet[i].path);
        }
        //foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        //{
        //    if (!scene.enabled)
        //        continue;
        //    EditorScenes.Add(scene.path);
        //}
        return EditorScenes.ToArray();
    }

    #region Editor
    [MenuItem("Custom/Version/CodeUp", false, 1)]
    static void CodeUp() 
    {
        int code = PlayerSettings.Android.bundleVersionCode;
        code += 1;
        PlayerSettings.Android.bundleVersionCode = code;
        //Debug.Log(" ===== Code ===== " + code );
    }

    [MenuItem("Custom/Build/Android")]
    static void AndroidBuild()
    {
        string BUILD_TARGET_PATH = TARGET_DIR + "/Android/";
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        PlayerSettings.companyName = "JunPortfolio";
        PlayerSettings.productName = "AshenVanguard";

        PlayerSettings.Android.keystoreName = Application.dataPath + "/user.keystore";
        PlayerSettings.Android.keystorePass = "junStorePass";
        PlayerSettings.Android.keyaliasName = "junAliasName";
        PlayerSettings.Android.keyaliasPass = "junAliasPass";
        PlayerSettings.bundleVersion = Application.version;

        string FileName = APP_NAME + ".apk";
        GenericBuild(SCENES, BUILD_TARGET_PATH + FileName, BuildTarget.Android, BuildOptions.None);
    }

    static void GenericBuild(string[] scenes, string filename, BuildTarget buildtarget, BuildOptions buildoption)
    {
        //EditorUserBuildSettings.SwitchActiveBuildTarget(buildtarget);
        BuildPipeline.BuildPlayer(scenes, filename, buildtarget, buildoption);
    }
    #endregion
}
