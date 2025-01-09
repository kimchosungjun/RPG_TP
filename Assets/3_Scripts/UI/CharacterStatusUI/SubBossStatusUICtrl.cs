using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBossStatusUICtrl : EliteMonsterStatusUI 
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
        if (playerStatusParentObject.activeSelf) playerStatusParentObject.SetActive(false);
    }

    // To Do ~~~ 
    // 보스용 스탯을 따로 만들어서 전달
    //public void Setups(Transform _followTransform = null)
    //{
    //    // Set
    //    hpImage.fillAmount = 1f;
    //    effectImage.fillAmount = 1f;
    //    levelText.text = "Lv." + 1;
    //    // Link
    //    followTransform = _followTransform;
    //    statusCanvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
    //    camTransform = Camera.main.transform;
    //}
    //public override void FixedExecute()
    //{
    //    UpdatePostion();

    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        hpImage.fillAmount -= 0.1f;
    //    }
    //    HPEffect();
    //}

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
    #endregion
}