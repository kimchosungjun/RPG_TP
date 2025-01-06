using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueReader
{
    public string ReadText(string _text, out bool _haveEvent)
    {
        string text = string.Empty;
        switch (_text[0])
        {
            case '$':
                _haveEvent = true;
                Event(_text);
                break;
            default:
                _haveEvent = false;
                text = _text;
                break;
        }
        return text;
    }

    public void Event(string _text)
    {
        int textLen = _text.Length;
        string eventType = string.Empty + _text[0] + _text[1];
        switch (eventType)
        {
            case "$Q":
                break;
            case "$A":
                break;
        }
    }

    public void Test()
    {
    }
}
