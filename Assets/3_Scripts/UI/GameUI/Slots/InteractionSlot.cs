using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSlot : MonoBehaviour
{
    [SerializeField] float fadeTime = 0.2f;
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

    IEnumerator CFadeIn()
    {
        float time = 0f;
        Color[] colors = new Color[4];
        for (int i = 0; i < 4; i++)
        {
            colors[i] = slotImages[i].color;
            colors[i].a = 1f;
            slotImages[i].color = colors[i];
        }
        while (time < fadeTime)
        {
            time += Time.fixedDeltaTime;
            for(int i=0; i<4; i++)
            {
                colors[i].a = Mathf.Lerp(1, 0, time / fadeTime);
                slotImages[i].color = colors[i];
            }
            yield return new WaitForFixedUpdate();  
        }

        for (int i = 0; i < 4; i++)
        {
            colors[i].a = 0f;
        }
    }

    IEnumerator CFadeOut()
    {
        float time = 0f;
        Color[] colors = new Color[4];
        for (int i = 0; i < 4; i++)
        {
            colors[i] = slotImages[i].color;
            colors[i].a = 0f;
            slotImages[i].color = colors[i];
        }
        while (time < fadeTime)
        {
            time += Time.fixedDeltaTime;
            for (int i = 0; i < 4; i++)
            {
                colors[i].a = Mathf.Lerp(0, 1, time / fadeTime);
                slotImages[i].color = colors[i];
            }
            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < 4; i++)
        {
            colors[i].a = 0f;
        }
    }
}
