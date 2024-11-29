using UnityEngine;
using UnityEngine.UI;

public abstract class MonsterStatusUICtrl : MonoBehaviour
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Link UI & Value
    [SerializeField] protected Canvas statusCanvas;
    [SerializeField] protected Image hpImage;
    [SerializeField] protected Image effectImage;
    [SerializeField] protected Text levelText;
    protected float effectTime = 5f;
    #endregion

    /******************************************/
    /********  몬스터에 의해 호출   ********/
    /******************************************/

    #region Life Cycle
    public abstract void Init();
    /// <summary>
    /// Setup에서 받는 것은 몬스터의 스텟으로 변경할 예정
    /// </summary>
    /// <param name="_followTransform"></param>
    /// <param name="_monsterHeight"></param>
    /// <param name="_level"></param>
    public abstract void Setup(Transform _followTransform, float _monsterHeight, int _level = 0);
    public abstract void Execute();
    #endregion

    /******************************************/
    /**********  캔버스 활성화   ************/
    /******************************************/
    public void DecideActiveCanvas(bool _isActive) { statusCanvas.gameObject.SetActive(_isActive); }
}


// 나중에 사용할 수도 있어서 보류
#region Atlas : Not Use
// 아틀라스로 묶일 때, 이미지 깨지는 현상때문에 아틀라스는 보류
//for (int i=0; i<2; i++) { hpImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "FullHP_0"); }
//for(int i=0; i<2; i++) { effectImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EffectHP_0"); }
//backImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EmptyHP_0");
#endregion