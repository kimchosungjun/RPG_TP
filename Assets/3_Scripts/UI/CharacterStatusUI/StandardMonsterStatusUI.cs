using UnityEngine;
using UnityEngine.UI;
using UIEnums;

public class StandardMonsterStatusUI : StatusUI
{
    /******************************************/
    /************* UI 위치 변수  ************/
    /******************************************/

    #region Variable
    [SerializeField, Tooltip("HP바의 위치 : 머리에서 좀 더 떨어지도록 설정")] float heightDelta = 0.2f;
    [SerializeField] protected Transform followTransform = null;
    [SerializeField] Image[] hpImages;
    [SerializeField] Image[] effectImages;
    protected float effectSpeed = 0.2f;
    protected Transform camTransform = null;
    protected MonsterStat monsterStat = null;
    
    bool isActive = false;
    #endregion

    /******************************************/
    /******* 라이프 사이클 재정의 *********/
    /******************************************/
    
    #region Override Life Cycle
    public override void Init()
    {
        DecideActiveState(false);
        hpImages[0].fillAmount = 1; 
        effectImages[0].fillAmount= 1;
    }

    public void Setup(Transform _followTransform, MonsterStat _monsterStat)
    {
        // Link
        monsterStat = _monsterStat;
        followTransform = _followTransform;
        playerStatusParentObject.GetComponent<Canvas>().worldCamera = Camera.main;
        camTransform = Camera.main.transform;

        Sprite hpLineSprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Loading_Line");
        hpImages[0].sprite = hpLineSprite;
        hpImages[1].sprite = hpLineSprite;
        effectImages[0].sprite = hpLineSprite;
        effectImages[1].sprite = hpLineSprite;

        // Set
        hpImages[0].fillAmount = 1f;
        effectImages[0].fillAmount = 1f;
        levelText.text = "Lv." + monsterStat.Level;
    }

    public virtual void FixedExecute()
    {
        if (isActive)
        {
            UpdatePostion();
            HPEffect();
        }
    }

    #endregion


    public void DecideActiveState(bool _isActive)
    {
        playerStatusParentObject.SetActive(_isActive);
        isActive = _isActive;

        if (isActive)
            UpdateStatusData();
    }

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
        if (effectImages[0].fillAmount > hpImages[0].fillAmount)
            effectImages[0].fillAmount -= Time.fixedDeltaTime * effectSpeed;
    }

    public override void UpdateStatusData()
    {
        hpImages[0].fillAmount =(float)monsterStat.CurrentHP / monsterStat.MaxHP;
    }

    #endregion
}
