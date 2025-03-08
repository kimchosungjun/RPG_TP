using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningCamControl : MonoBehaviour
{
    [SerializeField] GameObject[] camParents;
    void Start()
    {
        StartCoroutine(CRotateCam());
    }

    IEnumerator CRotateCam()
    {
        float time = 0;
        while (time < 5f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        camParents[1].SetActive(true);
        camParents[0].SetActive(false);
    }
}
