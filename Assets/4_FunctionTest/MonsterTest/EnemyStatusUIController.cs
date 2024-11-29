using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

enum E_STATUS_UI_HP
{
    MASK=0,
    FILL=1
}

public class EnemyStatusUIController : MonoBehaviour
{
    [SerializeField] Canvas statusCanvas;
    [SerializeField] GameObject hpUIObject;
    [SerializeField] Image hpImage;
    [SerializeField] Image effectImage;
    [SerializeField] Image backImage;
    [SerializeField] Text levelText;
    [SerializeField] float effectTime;
    Transform followTransform = null;
    Transform camTransform = null;
    public void Setup(Transform _followTransform, int _level = 0)
    {
        followTransform = _followTransform;
        levelText.text = "Lv." + _level;
        statusCanvas.worldCamera = Camera.main;
        camTransform = Camera.main.transform;
    }

    public void Execute()
    {
        transform.position = followTransform.position + Vector3.up;
        transform.rotation = camTransform.rotation;

        if (Input.GetKeyDown(KeyCode.F))
        {
            hpImage.fillAmount -= 0.1f;
        }
        HPEffect();
    }

    public void HPEffect()
    {
        if (hpImage.fillAmount == effectImage.fillAmount) return;

        if (hpImage.fillAmount < effectImage.fillAmount) effectImage.fillAmount -= Time.deltaTime / effectTime;
        else if(hpImage.fillAmount> effectImage.fillAmount) effectImage.fillAmount = hpImage.fillAmount;
    }

    //IEnumerator HitEffect()
    //{
    //    float time = 0f;
        
    //}


    public void DecideShowLevel(bool _isShow) { levelText.gameObject.SetActive(_isShow); }
    public void DecideShowHP(bool _isShow) { hpUIObject.SetActive(_isShow); }


}


// 아틀라스로 묶일 때, 이미지 깨지는 현상때문에 아틀라스는 보류
//for (int i=0; i<2; i++) { hpImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "FullHP_0"); }
//for(int i=0; i<2; i++) { effectImages[i].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EffectHP_0"); }
//backImage.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("AT_HPUI", "EmptyHP_0");