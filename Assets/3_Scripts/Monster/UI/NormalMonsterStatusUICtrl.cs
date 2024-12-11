using UnityEngine;

public class NormalMonsterStatusUICtrl : StatusUICtrl
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [SerializeField, Tooltip("HP바의 위치 : 머리에서 좀 더 떨어지도록 설정")] float heightDelta = 0.2f;
    protected Transform followTransform = null;
    protected Transform camTransform = null;
    #endregion

    /******************************************/
    /******* 라이프 사이클 재정의 *********/
    /******************************************/
    
    #region Override Life Cycle
    public override void Init()
    {
        if (statusCanvasObject.activeSelf) statusCanvasObject.SetActive(false);
    }

    public override void Setup(Transform _followTransform, int _level = 0)
    {
        // Set
        hpImage.fillAmount = 1f;
        effectImage.fillAmount = 1f;
        levelText.text = "Lv." + _level;
        // Link
        followTransform = _followTransform;
        statusCanvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
        camTransform = Camera.main.transform;
    }
    public override void Execute()
    {
        UpdatePostion();

        if (Input.GetKeyDown(KeyCode.F))
        {
            hpImage.fillAmount -= 0.1f;
        }
        HPEffect();
    }

    #endregion


    /******************************************/
    /***** HP UI 위치,회전값 최신화 ******/
    /******  HP 감소 이펙트 최신화 *******/
    /******************************************/
    #region Update Status Information
    public void UpdatePostion()
    {
        transform.position = followTransform.position + Vector3.up * heightDelta;
        transform.rotation = camTransform.rotation;
    }

    public void HPEffect()
    {
        if (hpImage.fillAmount == effectImage.fillAmount) return;

        if (hpImage.fillAmount < effectImage.fillAmount) effectImage.fillAmount -= Time.deltaTime / effectTime;
        else if (hpImage.fillAmount > effectImage.fillAmount) effectImage.fillAmount = hpImage.fillAmount;
    }
    #endregion
}
