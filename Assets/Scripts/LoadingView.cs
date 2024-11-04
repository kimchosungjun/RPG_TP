using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    [SerializeField] GameObject loadingView;
    public void EnableLoadingView() { Debug.Log("작동함"); loadingView.SetActive(true); }
    public void DisableLoadingView() { Debug.Log("작동끝"); loadingView.SetActive(false); }
}
