using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    e_SCENE currentScene = e_SCENE.SCENE_TITLE;
    AsyncOperation asyncOperation = null;

    public void Init()
    {
        SharedMgr.SceneMgr = this;    
    }

    public void LoadScene(e_SCENE _changeScene, bool _isLoading)
    {
        if (currentScene == _changeScene)
            return;

        currentScene = _changeScene;

        int _loadSceneIndex = Enums.GetIntValue(currentScene);
        if (_isLoading)
        {
            SceneManager.LoadScene(_loadSceneIndex);
        }
        else
        {

        }
    }

    public void LoadingScene(int _loadIndex) { StartCoroutine(CLoadingScene(_loadIndex)); }

    IEnumerator CLoadingScene(int _loadIndex)
    {
        asyncOperation = SceneManager.LoadSceneAsync(_loadIndex);
        yield break;
    }
}
