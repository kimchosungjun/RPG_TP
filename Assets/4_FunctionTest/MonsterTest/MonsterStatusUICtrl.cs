using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MonsterStatusUICtrl : MonoBehaviour
{
    /******************************************/
    /*****************  변수  *****************/
    /******************************************/
    #region Link UI & Value
    [SerializeField] Canvas statusCanvas;
    [SerializeField] GameObject hpUIObject;
    [SerializeField] Image hpImage;
    [SerializeField] Image effectImage;
    [SerializeField] Image backImage;
    [SerializeField] Text levelText;
    [SerializeField, Tooltip("HP바의 위치 : 머리에서 좀 더 떨어지도록 설정")] float heightDelta = 0.2f;

    Transform followTransform = null;
    Transform camTransform = null;
    float effectTime = 5f;
    float monsterHeight = 0f;
    #endregion


    public void Init()
    {
        if (statusCanvas.gameObject.activeSelf) statusCanvas.gameObject.SetActive(false);
        if (hpUIObject.gameObject.activeSelf) hpUIObject.gameObject.SetActive(false);
        if (levelText.gameObject.activeSelf) levelText.gameObject.SetActive(false);
    }

    public void Setup(Transform _followTransform, float _monsterHeight,int _level = 0)
    {
        // Set
        hpImage.fillAmount = 1f;
        effectImage.fillAmount = 1f;
        levelText.text = "Lv." + _level;
        monsterHeight = _monsterHeight + heightDelta;
        // Link
        followTransform = _followTransform;
        statusCanvas.worldCamera = Camera.main;
        camTransform = Camera.main.transform;
    }

    public void Execute()
    {
        UpdatePostion();

        if (Input.GetKeyDown(KeyCode.F))
        {
            hpImage.fillAmount -= 0.1f;
        }
        HPEffect();
    }

    /// <summary>
    /// 몬스터의 
    /// </summary>
    public void UpdatePostion()
    {
        transform.position = followTransform.position + Vector3.up * monsterHeight;
        transform.rotation = camTransform.rotation;
    }

    public void HPEffect()
    {
        if (hpImage.fillAmount == effectImage.fillAmount) return;

        if (hpImage.fillAmount < effectImage.fillAmount) effectImage.fillAmount -= Time.deltaTime / effectTime;
        else if(hpImage.fillAmount> effectImage.fillAmount) effectImage.fillAmount = hpImage.fillAmount;
    }


    public void DecideShowLevel(bool _isShow) { levelText.gameObject.SetActive(_isShow); }
    public void DecideShowHP(bool _isShow) { hpUIObject.SetActive(_isShow); }


}


// 아틀라스로 묶일 때, 이미지 깨지는 현상때문에 아틀라스는 보류
//for (int i=0; i<2; i++) { hpImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "FullHP_0"); }
//for(int i=0; i<2; i++) { effectImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EffectHP_0"); }
//backImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EmptyHP_0");