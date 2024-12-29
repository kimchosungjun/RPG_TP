using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] Text interactionText;
    [SerializeField] GameObject interactionObject;

    public void Active(string _text)
    {
        interactionText.text = _text;
        interactionObject.SetActive(true);  
    }

    public void InActive()
    {
        interactionObject.SetActive(false);  
    }
}
