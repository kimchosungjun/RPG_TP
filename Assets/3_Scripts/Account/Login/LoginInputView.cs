using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginInputView : MonoBehaviour
{
    [SerializeField, Tooltip("0:ID, 1:PW, 2:Login, 3:Create Account")] Image[] images;
    [SerializeField] InputField idInput;
    [SerializeField] InputField pwInput;
    [SerializeField] Text indicateText;
    [SerializeField] GameObject viewParent;

    float showTime = 0f;
    bool isShowWarnText = false;
    bool onceSetImage = false;

    public void Init()
    {
        idInput.text = "";
        pwInput.text = "";
        indicateText.text = "";
        if(viewParent.activeSelf)viewParent.SetActive(false);
        pwInput.interactable = true;
        idInput.interactable = true;
        if (onceSetImage == false)
            SetImage();
    }

    public void SetImage()
    {
        onceSetImage = true;
        Sprite barSprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Dialogue_Bar");
        Sprite btnSprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Button_Atlas", "Red_Frame");
        images[0].sprite = barSprite;
        images[1].sprite = barSprite;
        images[2].sprite = btnSprite;
        images[3].sprite = btnSprite;
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
            ShowWarnText("계정 정보가 없습니다.");
            return;
        }

        if (_value == _password)
        {
            // 로그인 성공
            SharedMgr.SceneMgr.SetPlayerAccount("ID", _id);
            SharedMgr.SceneMgr.SetPlayerAccount("PW", _password);
            idInput.interactable = false;
            pwInput.interactable = false;
            SharedMgr.UIMgr.LoginUICtrl.DoLobby();
            SharedMgr.UIMgr.LoginUICtrl.DoOpenGate();
            viewParent.SetActive(false);
            
        }
        else
        {
            idInput.text = "";
            pwInput.text = "";
            ShowWarnText("비밀번호가 일치하지 않습니다.");
        }
    }

    public void CreateAccount()
    {
        // 회원가입
        string _id = idInput.text;
        string _password = pwInput.text;
        
        if (SharedMgr.SceneMgr.IsExistID(_id)  || _id == string.Empty)
        {
            if (_password.Length == 0 || _id.Length == 0)
            {
                ShowWarnText("ID와 비밀번호은 빈칸일 수 없습니다.");
                return;
            }
            ShowWarnText("이미 존재하는 계정입니다!");
        }
        else
        {
            SharedMgr.SceneMgr.SetPlayerAccount(_id, _password);
            ShowWarnText( "회원가입에 성공했습니다.");
        }
    }

    public void ShowWarnText(string _text)
    {
        indicateText.text = _text;  
        if(isShowWarnText)
        {
            showTime = 0;
            return;
        }
        isShowWarnText = true;
        StartCoroutine(CShowWarnText());
    }

    IEnumerator CShowWarnText()
    {
        indicateText.gameObject.SetActive(true);
        while (true)
        {
            if(showTime >= 3f)
            {
                indicateText.gameObject.SetActive(false);
                isShowWarnText = false;
                break;
            }
            yield return null;
        }
    }
}
