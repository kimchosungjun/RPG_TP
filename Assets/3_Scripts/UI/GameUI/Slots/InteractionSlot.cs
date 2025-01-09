using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSlot : MonoBehaviour
{
    [SerializeField] Text descriptionText;
    [SerializeField, Header("0 : F Key, 1 : Chat Icon, 2 : Direction Icon, 3:TextBox")] Image[] slotImages;
    Interactable interactable = null;
    public void SetImage()
    {
        slotImages[0].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "FKey_Icon");
        slotImages[1].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Interact_Icon");
        slotImages[2].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Icon_Atlas", "Direction_Icon");
        slotImages[3].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("Bar_Atlas", "Dialogue_Bar");
    }

    public bool IsSameData(Interactable _interactable)
    {
        if (interactable == _interactable)
        {
            InActive();
            return true;
        }
        return false;
    }

    public void Active(Interactable _interactable)
    {
        interactable = _interactable;
        descriptionText.text = _interactable.Detect();
        this.transform.SetAsLastSibling();
        this.gameObject.SetActive(true);
    }

    public void InActive()
    {
        interactable = null;
        this.gameObject.SetActive(false);
        //#if UNITY_STANDALONE_WIN
#if UNITY_EDITOR
        InActiveDirection();
#endif
    }

    public void ActiveDirection()
    {
        slotImages[2].gameObject.SetActive(true);
    }

    public void InActiveDirection()
    {
        slotImages[2].gameObject.SetActive(false);
    }

    public void PressInteractSlot()
    {
        interactable?.Interact();
    }
}
