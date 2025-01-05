using UnityEngine;
using UnityEngine.UI;

public class InteractionSlot : MonoBehaviour
{
    [SerializeField] Text descriptionText;
    [SerializeField, Header("0 : F Key, 1 : Chat Icon, 2 : Direction Icon")] Image[] slotImages;
    Interactable interactable = null;

    public void SetImage()
    {
        // Atlas
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
        this.gameObject.SetActive(true);
    }

    public void InActive()
    {
        this.gameObject.SetActive(false);
        interactable = null;
        this.transform.SetAsLastSibling();
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
