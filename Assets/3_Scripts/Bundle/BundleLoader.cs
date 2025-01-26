using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ILoad());
    }

    IEnumerator ILoad()
    {
        AssetBundleCreateRequest async = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "character"));
        yield return async;
        AssetBundle local = async.assetBundle;
        if (local == null)
            yield break;

        AssetBundleRequest request = local.LoadAssetAsync<GameObject>("character_name");
        yield return request;
        var prefab = request.asset as GameObject;   
        Instantiate(prefab);
        local.Unload(true);
    }
}
