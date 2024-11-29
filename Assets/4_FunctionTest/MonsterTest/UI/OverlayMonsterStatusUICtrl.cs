using UnityEngine;
using UnityEngine.UI;

public class OverlayMonsterStatusUICtrl : MonsterStatusUICtrl
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Variable
    [SerializeField] Text monsterNameText;
    [SerializeField] protected Image gaugeImage;

    #endregion


    /******************************************/
    /******* 라이프 사이클 재정의 *********/
    /******************************************/

    #region Override Life Cycle
    public override void Init()
    {
        if (statusCanvas.gameObject.activeSelf) statusCanvas.gameObject.SetActive(false);
    }

    public override void Setup(Transform _followTransform, float _monsterHeight, int _level = 0)
    {
    }
    public override void Execute()
    {
    }

    #endregion
}
