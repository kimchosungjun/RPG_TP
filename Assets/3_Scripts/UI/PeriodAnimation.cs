using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PeriodAnimation : MonoBehaviour
{
    int currentComma = 0;
    [SerializeField] int commaCnt = 3;
    [SerializeField] Text text;
    string originalText = string.Empty;
    
    public void Init()
    {
        if(text==null)text = GetComponent<Text>();
        if(text!=null) originalText = text.text;
        if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
    }

    public void StartPeriodAnimation()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(CPeriodAnimation());
    }

    public void EndPeriodAnimation()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
        currentComma = 0;
    }
    
    IEnumerator CPeriodAnimation()
    {
        while (true)
        {
            currentComma += 1;
            if (currentComma >= commaCnt) currentComma = 0;
            string connectText = originalText;
            for(int i=0; i < currentComma; i++)
            {
                connectText += '.';
            }
            text.text = connectText;
            yield return null;
        }
    }   
}
