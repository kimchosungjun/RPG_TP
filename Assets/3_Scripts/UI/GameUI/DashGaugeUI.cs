using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashGaugeUI : MonoBehaviour
{
    enum DASH_COLOR
    {
        RED,
        ORANGE,
        YELLOW,
        GREEN
    }

    [SerializeField] Image dashGauge;
    [SerializeField] Color[] colors;
    public void ActiveGauge()
    {
        dashGauge.gameObject.SetActive(true);
    }

    public void InActiveGauge()
    {
        dashGauge.gameObject.SetActive(false);
    }

    public void SetGaugeAmount(float _amount)
    {
        dashGauge.fillAmount = _amount;
        CheckGaugeColor();
    }

    public void UseDashGauge(float _useAmount = 0.2f)
    {
        dashGauge.fillAmount = (dashGauge.fillAmount - 0.2f > 0f) ?
            dashGauge.fillAmount - 0.2f : 0f;
        ActiveGauge();
        CheckGaugeColor();
    }

    public void CheckGaugeColor()
    {
        if(dashGauge.fillAmount < 0.25f)
        {
            dashGauge.color = colors[(int)DASH_COLOR.RED];

        }
        else if(dashGauge.fillAmount < 0.5f)
        {
            dashGauge.color = colors[(int)DASH_COLOR.ORANGE];

        }
        else if (dashGauge.fillAmount < 0.75f)
        {
            dashGauge.color = colors[(int)DASH_COLOR.YELLOW];
        }
        else
        {
            dashGauge.color = colors[(int)DASH_COLOR.GREEN]; 
        }
    }
}
