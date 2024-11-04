using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] e_SCENE nextScene;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SharedMgr.SceneMgr.LoadScene(nextScene,true);
        }
    }
}
