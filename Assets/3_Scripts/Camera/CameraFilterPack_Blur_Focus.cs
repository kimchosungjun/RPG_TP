using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blur/Focus")]
public class CameraFilterPack_Blur_Focus : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;
    private Material SCMaterial;
    [Range(-1, 1)]
    public float CenterX = 0f;
    [Range(-1, 1)]
    public float CenterY = 0f;
    [Range(0, 10)]
    public float Size = 5f;
    [Range(0.12f, 64)]
    public float Eyes = 2f;

    public static float ChangeCenterX;
    public static float ChangeCenterY;
    public static float ChangeSize;
    public static float ChangeEyes;

    Transform MyPlayer2DPos;
    #endregion

    #region Properties
    Material material
    {
        get
        {
            if(SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion

    private void Awake()
    {
        //MyPlayer2DPos = SharedMgr.GameCtrlMgr.GetPlayerCtrl.transform;

        ChangeCenterX = CenterX;
        ChangeCenterY = CenterY;
        ChangeSize = Size;
        ChangeEyes = Eyes;
        SCShader = Shader.Find("CameraFilterPack/Blur_Focus");

        if(!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    // Unity Reserv Method
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(SCShader != null)
        {
            TimeX += Time.deltaTime;

            if (TimeX > 100)
                TimeX = 0;

            material.SetFloat("_TimeX", TimeX);
            material.SetFloat("_CenterX", CenterX);
            material.SetFloat("_CenterY", CenterY);

            float result = Mathf.Round(Size / 0.2f) * 0.2f;

            material.SetFloat("Size", result);
            material.SetFloat("Circle", Eyes);
            material.SetVector("_ScreenResolution", new Vector2(Screen.width, Screen.height));

            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    private void DieUpdate()
    {
        CenterX = 0f;
        CenterY = 0f;
        Size = ChangeSize;
        Eyes = ChangeEyes;
    }

    private void Update()
    {
        if(Application.isPlaying)
        {
            //if(SharedMgr.BattleMgr.GameOver)
            //{
            //    DieUpdate();
            //    return;
            //}

            CenterX = 0f;
            CenterY = 0f;
            Size = ChangeSize;
            Eyes = ChangeEyes;

#if UNITY_EDITOR
            if(Application.isPlaying != true)
            {
                SCShader = Shader.Find("CameraFilterPack/Blur_Focus");
            }
#endif
        }
    }

    private void OnDisable()
    {
        if(SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }
    }
}
