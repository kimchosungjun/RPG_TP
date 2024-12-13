using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerStatLoader : MonoBehaviour
{
    TableMgr tableMgr;
    [SerializeField] GameObject playerObjeet;
    [SerializeField] CameraController cameraController;
    private void Start()
    {
        tableMgr = SharedMgr.TableMgr;
        //tableMgr.LinkPlayerTable();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "플레이어 생성"))
        {
            if(playerObjeet.activeSelf == false)
            {
                playerObjeet.SetActive(true);
                cameraController.ChangeState();
            }
        }
    }
}
