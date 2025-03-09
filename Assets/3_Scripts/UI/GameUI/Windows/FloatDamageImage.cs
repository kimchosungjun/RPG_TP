using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatDamageImage : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Image[] numberImages;
    [SerializeField, Range(0,5f)] float floatTime = 3f;
    [SerializeField] float floatSpeed = 6f;

    private void Awake()
    {
        canvas.worldCamera = Camera.main;
    }

    public void ShowImage(int _number, int _len)
    {
        int num = _number;
        int len = _len;

        int div = 0;
        while (len>=0)
        {
            div = num % 10;
            num /= 10;
            string numberText = div.ToString();
            numberImages[len].sprite = SharedMgr.ResourceMgr.GetSpriteAtlas("NumberFont_Atlas", numberText);
            len--;
        }
        StartCoroutine(CShowFloatDamage());
    }

    IEnumerator CShowFloatDamage()
    {
        float time = 0;
        Transform camTransform = Camera.main.transform;
        while(time < floatTime)
        {
            this.transform.rotation = camTransform.rotation;
            this.transform.position += Vector3.up * Time.fixedDeltaTime * floatSpeed;
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.gameObject.SetActive(false);
    }
}
