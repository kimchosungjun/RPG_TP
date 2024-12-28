using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatDamageTextUI : MonoBehaviour
{
    Vector3 offSetVec = Vector3.up;
    [SerializeField] TextMesh textMesh;
    [SerializeField] float floatTime = 1f;
    [SerializeField] float floatSpeed = 3f;
    public void SetFloat(Transform _objectTransform, float _damage)
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = _objectTransform.position + offSetVec;
        textMesh.text = ((int)_damage).ToString();

        StartCoroutine(CFloat());
    }

    IEnumerator CFloat()
    {
        float time = 0f;
        while (time < floatTime)
        {
            time += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime * floatSpeed;
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
