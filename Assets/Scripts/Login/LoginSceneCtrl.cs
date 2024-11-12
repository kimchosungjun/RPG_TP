using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSceneCtrl : MonoBehaviour
{
    [SerializeField] LoginUICtrl loginUICtrl;

    // To Do ~~
    // 1. 페이드인
    // 2. 리소스 파일 다운
    // 3. 화면 전환
    // 4. 로그인 정보 확인
    // 4-1 : 없으면 로그인
    // 4-2 : 있으면 터치 후 바로 게임 씬으로 이동
    // => 2,3은 제외

    // Flow
    // 1. 페이드인 효과 실행
    // 2. 로그인 정보 확인
    //   => 2-1. 로그인 정보가 없다면 로그인 UI 표시
    // 3. 로그인 확인 텍스트 출력 후, 로그인 UI 비활성화
    // 4. 화면 터치하면 (지금은 마우스 클릭) 로딩 씬 => 게임 씬으로 이동

    private void Awake()
    {
        if (loginUICtrl == null)
            loginUICtrl = FindObjectOfType<LoginUICtrl>();
    }


    void Start()
    {
        loginUICtrl.DoFadeIn();
        if(SharedMgr.SceneMgr.IsMaintainLogin())
        {
            // 게임 씬 동작 직전으로 이동
            loginUICtrl.DoLobby();
        }
        else
        {
            // 로그인 UI 활성화
            loginUICtrl.DoLogin();
        }
    }

    
}
