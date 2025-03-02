using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : UIBase
{
    [SerializeField] InputField talkInput;
    
    public void SendText()
    {
        string text = talkInput.text;
    }
}
