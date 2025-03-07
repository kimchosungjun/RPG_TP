using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] Text[] chatTexts;
    [SerializeField] InputField inputChat;
 
    public void SendChat()
    {
        if (inputChat.text != string.Empty)
        {
            SharedMgr.PhotonMgr.DoChat(inputChat.text);
            inputChat.text = string.Empty;
        }
    }

    public void DoChat(string _chatText)
    {
        int chatCnt = chatTexts.Length;
        for(int i=0; i<chatCnt; i++)
        {
            if(chatTexts[i].text == string.Empty)
            {
                chatTexts[i].text = _chatText;
                return;
            }
        }

        for(int k=1; k<chatCnt; k++)
        {
            chatTexts[k-1].text = chatTexts[k].text;
        }
        chatTexts[chatCnt - 1].text = _chatText;
    }


}
