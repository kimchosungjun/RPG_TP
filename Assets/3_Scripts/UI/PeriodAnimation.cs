using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PeriodAnimation : MonoBehaviour
{
    int currentComma = 0;
    [SerializeField] int commaCnt = 4;
    [SerializeField] Text text;
    string originalText = string.Empty;
    WaitForSeconds periodAnimTime = new WaitForSeconds(0.2f);

    public void Init()
    {
        if(text==null)text = GetComponent<Text>();
        if(text!=null) originalText = text.text;
        if (this.gameObject.activeSelf) this.gameObject.SetActive(false);
    }

    public void StartPeriodAnimation(UnityAction _action)
    {
        this.gameObject.SetActive(true);
        if (_action != null) StartCoroutine(CEnterAction(_action));
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
            yield return periodAnimTime;
        }
    }
    
    IEnumerator CEnterAction(UnityAction _action)
    {
        yield return new WaitForSeconds(2f);
        _action();
    }
}
