using System.Collections;
using UtilEnums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Photon.Pun;

public partial class SceneMgr : MonoBehaviour
{
    SCENES nextLoadScene = SCENES.LOADING;
    SCENES currentScene = SCENES.TITLE;
    AsyncOperation asyncOperation = null;
    public void Init() { SharedMgr.SceneMgr = this; }

    public bool IsLoginScene()
    {
        if (currentScene == SCENES.LOGIN)
            return true;
        return false;
    }

    public void LoadScene(SCENES _changeScene, bool _isLoading = false)
    {
        if (currentScene == _changeScene)
            return;

        currentScene = _changeScene;

        int _loadSceneIndex = Enums.GetIntValue(currentScene);
        if (!_isLoading)
            SceneManager.LoadScene(_loadSceneIndex);
        else
        {
            if(nextLoadScene == _changeScene)       
            {
                Debug.LogError("Scene Load Error!! : Same Scene Load");
                return;
            }
            nextLoadScene = _changeScene;
            SceneManager.LoadScene((int) SCENES.LOADING);
        }
    }


    /// <summary>
    ///  Call By Loading Scene : Loading UI
    /// </summary>
    public void LoadingScene(UnityAction _action = null) { StartCoroutine(CLoadingScene((int)nextLoadScene, _action)); }

    IEnumerator CLoadingScene(int _loadIndex, UnityAction _action = null)
    {
        if (nextLoadScene == SCENES.GAME)
            PhotonNetwork.LoadLevel(_loadIndex, ref asyncOperation);
        else
            asyncOperation = SceneManager.LoadSceneAsync(_loadIndex);

        LoadingUICtrl _uiController = SharedMgr.UIMgr.LoadingUICtrl;
        float fakeProgress = 0f;
        float realProgress = 0f;
        
        // Fake Loading
        while (fakeProgress < 0.9f)
        {
            realProgress = asyncOperation.progress;
            fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime);
            _uiController.UpdateLoadingPercent(fakeProgress);
            yield return null;
        }

        currentScene = nextLoadScene;
        nextLoadScene = SCENES.LOADING;

        asyncOperation.allowSceneActivation = true; 

        if (_action!=null)
            _action();   
    }



    public bool CheckEndSceneLoad() 
    { 
        if (asyncOperation.isDone)
        {
            asyncOperation = null; 
            return true; 
        } 
        else
            return false; 
    }
}
