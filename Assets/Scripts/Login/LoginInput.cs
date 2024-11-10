using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginInput : MonoBehaviour
{
    [SerializeField] InputField idInput;
    [SerializeField] InputField pwInput;
    [SerializeField] Text text;

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
    }

    public void PressLoginBtn()
    {
        string _id = idInput.text;
        string _password = pwInput.text;

        string _value = SharedMgr.SceneMgr.GetPlayerPrefersStringKey(_id);
        Debug.Log(_value);
        if (_value == string.Empty)
        {
            idInput.text = "";
            pwInput.text = "";
            text.text = "로그인 실패!";
            return;
        }

        if (_value == _password)
        {
            text.text = "로그인 성공!";
        }
        else
        {
            idInput.text = "";
            pwInput.text = "";
            text.text = "로그인 실패!";
        }
    }

    public void CreateAccount()
    {
        text.text = "회원가입 성공!";
        string _id = idInput.text;
        string _password = pwInput.text;
        SharedMgr.SceneMgr.SetPlayerPrefersIntKey(_id, _password);
    }
}
