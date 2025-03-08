using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class JoystickUI : MonoBehaviour
{
    [SerializeField] Image stick;
    Vector3 input = Vector3.zero;
    Vector3 position = Vector3.zero;

    public void PressOnDown(BaseEventData eventData)
    {
#if UNITY_ANDROID
        stick.gameObject.SetActive(true);
        Touch touch = Input.GetTouch(0);
        transform.position = touch.position;
        OnDown((PointerEventData)eventData);
#endif
    }
    public void PressOnUp(BaseEventData eventData)
    {
        OnUp((PointerEventData)eventData);
    }
    public void PressOnDrag(BaseEventData eventData)
    {
        //stick.rectTransform.position = Input.mousePosition;
        //Touch touch = Input.GetTouch(0);
        //transform.position = touch.position;
        OnDown((PointerEventData)eventData);
    }


    public void OnDown(PointerEventData eventData)
    {
        OnDrag((PointerEventData)eventData);
    }

    public void OnUp(PointerEventData eventData)
    {
        input = Vector3.zero;
        stick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(stick.rectTransform, eventData.position,
            eventData.pressEventCamera, out Vector2 localPoint))
        {
            localPoint.x = localPoint.x / stick.rectTransform.sizeDelta.x;
            localPoint.y = localPoint.y / stick.rectTransform.sizeDelta.y;
            input.x = localPoint.x;
            input.y = localPoint.y;

            input = (input.magnitude > 1.0f) ? input.normalized : input;
            position.x = input.x * stick.rectTransform.sizeDelta.x / 2f;
            position.y = input.y * stick.rectTransform.sizeDelta.y / 2f;
            stick.rectTransform.anchoredPosition = position;
        }
    }

    public float InputX() { return input.x; }
    public float InputZ() { return input.z; }   
}
