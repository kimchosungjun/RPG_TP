using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginInputView : MonoBehaviour
{
    [SerializeField] InputField idInput;
    [SerializeField] InputField pwInput;
    [SerializeField] Text indicateText;
    [SerializeField] GameObject viewParent;
    public void Init()
    {
        idInput.text = "";
        pwInput.text = "";
        indicateText.text = "";
        if(viewParent.activeSelf)viewParent.SetActive(false);
    }

    public void ActiveView() { viewParent.SetActive(true); }

    public void PressLoginBtn()
    {
        string _id = idInput.text;
        string _password = pwInput.text;
        string _value = SharedMgr.SceneMgr.GetPlayerAccount(_id);

        if (SharedMgr.SceneMgr.IsExistID(_id) == false)
        {
            idInput.text = "";
            pwInput.text = "";
            indicateText.text = "로그인 실패!";
            return;
        }

        if (_value == _password)
        {
            // 로그인 성공
            SharedMgr.SceneMgr.SetPlayerAccount("ID", _id);
            SharedMgr.SceneMgr.SetPlayerAccount("PW", _password);
            idInput.interactable = false;
            pwInput.interactable = false;
            GetComponentInParent<LoginUICtrl>().DoLobby();
            viewParent.SetActive(false);
        }
        else
        {
            idInput.text = "";
            pwInput.text = "";
            indicateText.text = "로그인 실패!";
        }
    }

    public void CreateAccount()
    {
        // 회원가입
        string _id = idInput.text;
        string _password = pwInput.text;
        if (SharedMgr.SceneMgr.IsExistID(_id))
        {
            if (_password.Length == 0 || _id.Length == 0)
            {
                indicateText.text = "ID와 비밀번호은 빈칸일 수 없습니다.";
                return;
            }
            indicateText.text = "이미 존재하는 계정입니다!";
        }
        else
        {
            SharedMgr.SceneMgr.SetPlayerAccount(_id, _password);
            indicateText.text = "회원가입 성공!";
        }
    }
}
