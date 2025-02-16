using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;


public class Capture : MonoBehaviour
{
    public enum Size
    {
        POT64,
        POT128,
        POT256,
        POT512,
        POT1024,
    }

    public Size size;
    public Camera cam;
    public RenderTexture render;
    public Image img;
    string extention = ".png";
    [SerializeField] string filename;

    public GameObject[] objs;
    int cnt = 0;

    public void Start()
    {
        cam = Camera.main;
        SetSize();
    }

    public void Create()
    {
        StartCoroutine(CaptureImage());
    }


    IEnumerator CaptureImage()
    {
        yield return null;  
        Texture2D tex = new Texture2D(render.width, render.height, TextureFormat.ARGB32, false,true);
        RenderTexture.active = render;
        tex.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
        yield return null;

        var data = tex.EncodeToPNG();
        string path = Application.dataPath + "/Capture/";
        Debug.Log(path);

        if(!Directory.Exists(path)) Directory.CreateDirectory(path);
        File.WriteAllBytes(path+filename+extention,data);
        yield return null;
    }

    public void AllCreate()
    {
        StartCoroutine(AllCaptureImage());
    }


    IEnumerator AllCaptureImage()
    {
        while (cnt < objs.Length)
        {
            var obj = Instantiate(objs[cnt].gameObject);
            
            yield return null;
            
            Texture2D tex = new Texture2D(render.width, render.height, TextureFormat.ARGB32, false, true);
            RenderTexture.active = render;
            tex.ReadPixels(new Rect(0, 0, render.width, render.height), 0, 0);
            yield return null;

            var data = tex.EncodeToPNG();
            string path = Application.dataPath + "/Capture/";
            Debug.Log(path);

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllBytes(path + obj.name + extention, data);
            yield return null;
            DestroyImmediate(obj);
            cnt++;

            yield return null;  
        }
    }

    void SetSize()
    {
        switch (size)
        {
            case Size.POT64:
                render.width = 64;
                render.height = 64;
                break;
            case Size.POT128:
                render.width = 128;
                render.height = 128;
                break;
            case Size.POT256:
                render.width = 256;
                render.height = 256;
                break;
            case Size.POT512:
                render.width = 512;
                render.height = 512;
                break;
            case Size.POT1024:
                render.width = 1024;
                render.height = 1024;
                break;
        }
    }
}
