using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    [SerializeField] GameObject loadingView;

    public void Init()
    {
        if (loadingView == null)
        {
            Transform[] childrenTfs = GetComponentsInChildren<Transform>();
            loadingView = childrenTfs[1].gameObject;
        }
    }

    public void EnableLoadingView() { loadingView.SetActive(true); }
    public void DisableLoadingView() { loadingView.SetActive(false); }
}
