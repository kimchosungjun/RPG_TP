using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIDUI : MonoBehaviour
{
    [SerializeField] Text playerIDText;

    public void SetText(string _ID)
    {
        playerIDText.text = _ID;
    }
}
