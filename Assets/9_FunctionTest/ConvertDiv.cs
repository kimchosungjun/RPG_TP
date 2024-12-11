using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertDiv : MonoBehaviour
{
    [SerializeField, TextArea] string defaultStr;
    [SerializeField, TextArea] string converstStr;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "변환"))
        {
            converstStr = Convert(defaultStr);
        }
    }

    public string Convert(string str)
    {
        str = str.Replace(",", "|").Replace(" ", "");
        return str;
    }
}
