using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimeLineCanvas : MonoBehaviour
{

    [SerializeField] PlayableDirector playableDirector;
    [SerializeField] Image img;
    
    void Start()
    {
        playableDirector.stopped += OnTimelineStopped;
    }

    IEnumerator CFade()
    {
        Color color = Color.black;
        color.a = 0f;
        img.color = color;

        float time = 0;
        while (time < 1f)
        {
            color.a = Mathf.Lerp(0, 1, time / 1f);
            img.color = color;
            time += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        img.color = color;
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        director.Play(); // 타임라인이 종료되면 다시 재생
    }
}
