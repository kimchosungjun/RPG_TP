using UnityEngine;
using UnityEngine.UI;
using UIEnums;

public class EliteMonsterStatusUI : StatusUI
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [Header("Elite Monster")]
    [SerializeField] protected Text monsterNameText;
    [SerializeField] protected Image gaugeImage;
    [SerializeField, Tooltip("게이지 색을 바꾸기 위해 필요")] protected Image fillGaugeImage;
    [SerializeField] protected Color groggyColor;
    protected Color defaultColor = Color.white;
    #endregion


    /******************************************/
    /******* 라이프 사이클 재정의 *********/
    /******************************************/

    #region Override Life Cycle
    public override void Init()
    {
        if (statusCanvasObject.activeSelf) statusCanvasObject.SetActive(false);
    }

    public void Setup(Transform _followTransform = null)
    {

    }

    public override void FixedExecute()
    {

    }

    public override void AnnounceChangeStat(STATUS _statusType = STATUS.HP)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
