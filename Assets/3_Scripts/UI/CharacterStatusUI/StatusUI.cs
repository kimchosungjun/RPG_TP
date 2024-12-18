using UnityEngine;
using UnityEngine.UI;
using UIEnums;

public abstract class StatusUI : MonoBehaviour
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/

    #region Link UI & Value
    [Header("Monster")]
    [SerializeField] protected GameObject statusCanvasObject;
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
    public abstract void FixedExecute();
    #endregion

    /******************************************/
    /**********  캔버스 활성화   ************/
    /******************************************/
    public void DecideActiveCanvas(bool _isActive) { statusCanvasObject.gameObject.SetActive(_isActive); }
    public virtual void HPEffect()
    {
        if (hpImage.fillAmount == effectImage.fillAmount) return;

        if (hpImage.fillAmount < effectImage.fillAmount) effectImage.fillAmount -= Time.deltaTime / effectTime;
        else if (hpImage.fillAmount > effectImage.fillAmount) effectImage.fillAmount = hpImage.fillAmount;
    }
    public abstract void AnnounceChangeStat(STATUS _statusType = STATUS.HP);
}


// 나중에 사용할 수도 있어서 보류
#region Atlas : Not Use
// 아틀라스로 묶일 때, 이미지 깨지는 현상때문에 아틀라스는 보류
//for (int i=0; i<2; i++) { hpImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "FullHP_0"); }
//for(int i=0; i<2; i++) { effectImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EffectHP_0"); }
//backImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EmptyHP_0");
#endregion