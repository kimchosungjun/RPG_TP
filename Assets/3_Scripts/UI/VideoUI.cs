using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoUI : MonoBehaviour
{
    [SerializeField] VideoPlayer player;
    [SerializeField] RawImage rawImg;
    VideoClip clip = null;
    string fileName = "Opening";

    public void SetVideo(UnityAction _action = null)
    {
        Color color = Color.white;
        color.a = 0;
        rawImg.color = color;
        string file = "Prefabs/Video/" + fileName;
        clip = Resources.Load(file) as VideoClip;
        if (clip == null)
        {
            Debug.LogError("No Video!! Error!!");
            if (_action != null)
                _action();
            return;
        }

        player.gameObject.SetActive(true);
        rawImg.texture = player.texture;
        player.clip = clip;
        player.Prepare();
        StartCoroutine(UpdateVideo(_action));
    }

    // 서버와의 데이터 통신이 느린 경우가 발생 : 코루틴을 이용해서 비디오를 반복재생
    // 서버와의 데이터 통신이 끝나면 다음 영상 재생

    IEnumerator UpdateVideo(UnityAction _action = null)
    {
        //1. 비디오 재생
        //2. waitforsecond(0.1f)
        //3. while문 실행
        //4. 비디오 재생 & 조건에 맞춰 비디오 변경
        player.Play();
        yield return new WaitForSeconds(0.1f);
        Color color = Color.white;
        color.a = 1;
        rawImg.color = color;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (player.isPlaying)
            {
                rawImg.texture = player.texture;
                continue;
            }
            break;
        }
        player.gameObject.SetActive(false);
        if (_action != null)
            _action();
    }
}
