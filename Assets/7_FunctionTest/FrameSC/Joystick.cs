using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
    [SerializeField] Transform tf;
    public Image ball;
    Vector3 input = Vector3.zero;
    Vector3 position = Vector3.zero;

   public void PressOnDown(BaseEventData eventData)
    {
        ball.gameObject.SetActive(true);
#if UNITY_EDITOR
        tf.position = Input.mousePosition;
#else
        Touch touch = Input.GetTouch(0);
        transform.position = touch.position;
#endif
        OnDown((PointerEventData)eventData);
    }
    public void PressOnUp(BaseEventData eventData) 
    {
        ball.gameObject.SetActive(false);
        OnUp((PointerEventData)eventData);
    }
   public void PressOnDrag(BaseEventData eventData) 
    {
        //ball.rectTransform.position = Input.mousePosition;
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
        ball.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(ball.rectTransform, eventData.position, 
            eventData.pressEventCamera, out Vector2 localPoint))
        {
            localPoint.x = localPoint.x / ball.rectTransform.sizeDelta.x;
            localPoint.y = localPoint.y / ball.rectTransform.sizeDelta.y;
            input.x = localPoint.x;
            input.y = localPoint.y;

            //3항연산자 : 연산자가 가장 빠르기 때문에 사용을 추천한다.
            input = (input.magnitude > 1.0f) ? input.normalized : input;
            position.x = input.x * ball.rectTransform.sizeDelta.x/2f;
            position.y = input.y * ball.rectTransform.sizeDelta.y/2f;
            ball.rectTransform.anchoredPosition = position; 

            // 이동
        }
    }
}
