using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IndicatorUI : MonoBehaviour, ICommonSetUI
{
    [SerializeField] GameObject allIndicatorParent;
    public void Init()
    {
        SetImages();
    }

    public void SetImages()
    {
        ResourceMgr res = SharedMgr.ResourceMgr;
        healIndicatorImage.sprite = res.GetSpriteAtlas("Bar_Atlas", "Gold_Bar");
        gameOverImages[0].sprite = res.GetSpriteAtlas("Window_Atlas", "Popup_Frame");
        gameOverImages[1].sprite = res.GetSpriteAtlas("Button_Atlas", "Red_Frame");

        minimapImages[0].sprite = res.GetSpriteAtlas("Minimap_Atlas", "MiniBack");
        minimapImages[1].sprite = res.GetSpriteAtlas("Minimap_Atlas", "MiniFrame");
        minimapImages[2].sprite = res.GetSpriteAtlas("Minimap_Atlas", "PlusBtn");
        minimapImages[3].sprite = res.GetSpriteAtlas("Minimap_Atlas", "MinutBtn");
    }

    #region Heal Indicator
    [Header("Heal Indicator")]
    [SerializeField] GameObject healIndicatorObject;
    [SerializeField] Image healIndicatorImage;
    [SerializeField] Animator healIndicatorAnim;
    bool isActiveHealIndicator = false;
    float timer = 0f;
    Coroutine healCor = null;

    public void ActiveHealIndicator()
    {
        if (isActiveHealIndicator == false)
            healCor = StartCoroutine(CHealIndicate());
        else
        {
            if (healIndicatorImage.gameObject.activeSelf == false)
            {
                if(healCor!=null)
                    StopCoroutine(healCor); 
                StartCoroutine(CHealIndicate());
                return;
            }
            timer = 0f;
        }
    }


    IEnumerator CHealIndicate()
    {
        timer = 0f;
        isActiveHealIndicator = true;
        healIndicatorObject.SetActive(true);
        healIndicatorAnim.SetInteger("State", 1);
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.HEAL_SFX);
        while (timer < 3f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        isActiveHealIndicator = false;
        healIndicatorAnim.SetInteger("State", 2);
        healCor = null;
    }
    #endregion

    #region Game Over Indicator
    [Header("Game Over")]
    [SerializeField] Image[] gameOverImages;
    [SerializeField] GameObject gameOverObject;

    public void ActiveGameOver()
    {
        SharedMgr.CursorMgr.SetCursorVisibleState(true);
        gameOverObject.SetActive(true);
        SharedMgr.SoundMgr.PlaySFX(UtilEnums.SFXCLIPS.PLAYERALL_DEATH_SFX);
    }

    public void PressConfirmGameOver()
    {
        SharedMgr.CursorMgr.SetCursorVisibleState(false);
        SharedMgr.SoundMgr.PressButtonSFX();
        gameOverObject.SetActive(false);
        FadeOut(SharedMgr.GameCtrlMgr.GetPlayerCtrl.MoveToNearTown);
    }
    #endregion

    #region Fade
    [Header("Fade")]
    [SerializeField] float fadeTimer = 2f;
    [SerializeField] Image fadeImage;

    bool isFadeOut = false;
    public void FadeOut(UnityAction _action = null)
    {
        if (isFadeOut == false)
        {
            isFadeOut = true;
            StartCoroutine(CFade(0,1, _action));
        }
    }

    /// <summary>
    /// start, end (0,1 : FadeOut) / start, end(1,0 : Fade In)
    /// </summary>
    /// <param name="_start"></param>
    /// <param name="_end"></param>
    /// <returns></returns>
    IEnumerator CFade(int _start, int _end, UnityAction _action = null)
    {

        float fadeTime = 0;
        Color defaultColor = Color.black;
        defaultColor.a = _start;
        fadeImage.color = defaultColor;
        fadeImage.gameObject.SetActive(true);
        while (fadeTime < fadeTimer)
        {
            defaultColor.a = Mathf.Lerp(_start, _end, fadeTime / fadeTimer);
            fadeImage.color = defaultColor;
            fadeTime += Time.deltaTime; 
            yield return null;
        }

        defaultColor.a = _end;
        fadeImage.color = defaultColor;

        if (_action != null)
            _action();  
    }

    public void FadeIn(UnityAction _action = null)
    {
        _action += InActviveFade;

        if (isFadeOut == true)
        {
            isFadeOut = false;
            StartCoroutine(CFade(1, 0, _action));
        }
    }

    public void InActviveFade() { fadeImage.gameObject.SetActive(false); }


    #endregion

    #region Interface : Turn On & Off
    public void TurnOn()
    {
        allIndicatorParent.SetActive(true);
    }

    public void TurnOff()
    {
        allIndicatorParent.SetActive(false);
        if (healIndicatorImage.gameObject.activeSelf)
        {
            healIndicatorImage.gameObject.SetActive(false);
            healCor = null;
        }
    }
    #endregion

    #region Minimap
    [Header("Minimap")]
    [SerializeField, Tooltip("0 :Background, 1:Frame, 2:+Btn, 3:-Btn")] Image[] minimapImages;
    
    public void PressZoomIn() { SharedMgr.GameCtrlMgr.GetCameraCtrl.Zoom(); }
    public void PressZoomOut() { SharedMgr.GameCtrlMgr.GetCameraCtrl.Zoom(false); }
    #endregion
}
