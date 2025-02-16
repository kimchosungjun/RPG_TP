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
    static string APP_NAME = "kks";
    //static int BUILD_DATE = 0;

    static string[] FindEnableEditorScenes()
    {
        List<string> EditorScenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;

            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    [MenuItem("Custom/Version/CodeUp", false, 1)]
    static void CodeUp() 
    {
        int code = PlayerSettings.Android.bundleVersionCode;

        code += 1;

        PlayerSettings.Android.bundleVersionCode = code;

        Debug.Log(" ===== Code ===== " + code );
    }

    [MenuItem("Custom/Build/Android")]
    static void AndroidBuild()
    {
        string BUILD_TARGET_PATH = TARGET_DIR + "/Android/";
        
        Directory.CreateDirectory(BUILD_TARGET_PATH);

        PlayerSettings.companyName = "ksgames";
        PlayerSettings.productName = "kks";

        PlayerSettings.Android.keystoreName = Application.dataPath + "/user.keystore";
        PlayerSettings.Android.keystorePass = "kkskks";
        PlayerSettings.Android.keyaliasName = "kks";
        PlayerSettings.Android.keyaliasPass = "kkskks";

        PlayerSettings.bundleVersion = Application.version;

        string FileName = APP_NAME + ".apk";

        GenericBuild(SCENES, BUILD_TARGET_PATH + FileName, BuildTarget.Android, BuildOptions.None);
    }

    static void GenericBuild(string[] scenes, string filename, BuildTarget buildtarget, BuildOptions buildoption)
    {
        //EditorUserBuildSettings.SwitchActiveBuildTarget(buildtarget);

        BuildPipeline.BuildPlayer(scenes, filename, buildtarget, buildoption);
    }
}
