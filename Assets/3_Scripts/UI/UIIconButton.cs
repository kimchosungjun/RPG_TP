using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIIconButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected Vector3 originalScale;
    protected float scaleIncreasment = 1.1f;
    [SerializeField] bool isExitButton = false;
    [SerializeField] protected Transform btnTransform;
    [SerializeField] protected Button btn;
    [SerializeField] string iconName;
    [SerializeField] GameUICtrl.InputKeyUITypes inputKeyType;

    protected virtual void Awake()
    {
        originalScale = btnTransform.localScale;

        if (btnTransform == null)
            btnTransform = GetComponent<Transform>();
        if (btn == null)
            btn = GetComponent<Button>();
        SettingImages();
    }

    protected void SettingImages()
    {
        Image img = btn.GetComponent<Image>();
        if (isExitButton)
        {
            img.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_4", iconName);
            return;
        }
        img.sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas_5", iconName);    
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!btn.interactable)
            return;
        btnTransform.localScale = originalScale * scaleIncreasment;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!btn.interactable)
            return;
        btnTransform.localScale = originalScale;
    }

    public void ResizeButtonScale(bool _isIncrease = true)
    {
        if (_isIncrease)
        {
            btnTransform.localScale = originalScale * scaleIncreasment;
        }
        else
        {
            btnTransform.localScale = originalScale;
        }
    }

    public void PressIcon()
    {
        SharedMgr.UIMgr.GameUICtrl.InputUIIcon(inputKeyType);
    }
}
