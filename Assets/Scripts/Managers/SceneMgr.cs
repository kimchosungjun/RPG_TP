using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    E_SCENE CURRENT_SCENE = E_SCENE.SCENE_TITLE;
    AsyncOperation asyncOperation = null;
    CommonUIController commonUIController = null;
    public CommonUIController CommonUIController { set { commonUIController = value; } }

    public void Init()
    {
        SharedMgr.SceneMgr = this;
        SceneManager.LoadScene(Enums.GetIntValue(E_SCENE.SCENE_UI), LoadSceneMode.Additive);
    }

    public void LoadScene(E_SCENE _changeScene, bool _isLoading = false)
    {
        if (CURRENT_SCENE == _changeScene)
            return;

        CURRENT_SCENE = _changeScene;

        int _loadSceneIndex = Enums.GetIntValue(CURRENT_SCENE);
        if (!_isLoading)
            SceneManager.LoadScene(_loadSceneIndex);
        else
            commonUIController.UpdateFade(false, LoadingScene);
    }

    /// <summary>
    /// 페이드 아웃 후 실행
    /// </summary>
    public void LoadingScene()  { StartCoroutine(CLoadingScene((int)CURRENT_SCENE));  }

    IEnumerator CLoadingScene(int _loadIndex)
    {
        asyncOperation = SceneManager.LoadSceneAsync(_loadIndex);
        asyncOperation.allowSceneActivation = false;

        // 로딩 UI 실행
        commonUIController.UpdateLoading(true);

        float fakeProgress = 0f;
        float realProgress = 0f;
        
        // 로딩 시간을 의도적으로 늘림
        while (fakeProgress < 0.9f)
        {
            realProgress = asyncOperation.progress;
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime);
            yield return null;
        }

        asyncOperation.allowSceneActivation = true;

        // 로딩 UI 중지, 페이드 인 실행
        commonUIController.UpdateLoading(false);
        commonUIController.UpdateFade(true);
    }
}
