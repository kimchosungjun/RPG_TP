using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoView : MonoBehaviour
{
    [SerializeField] string fileName ="EF_Normal";
    public VideoPlayer player;
    public RawImage rawImg;
    VideoClip clip = null;

    void SetVideo()
    {
        string file = "Prefabs/Video/" + fileName;
        clip = Resources.Load(file) as VideoClip;
        if (clip == null)
        {
            Debug.LogError("해당 비디오를 찾지 못했습니다.");
        }

        player.gameObject.SetActive(true);
        rawImg.texture = player.texture;
        player.clip = clip;
        player.Prepare();
        StartCoroutine(UpdateVideo());
    }

    // 서버와의 데이터 통신이 느린 경우가 발생 : 코루틴을 이용해서 비디오를 반복재생
    // 서버와의 데이터 통신이 끝나면 다음 영상 재생

    IEnumerator UpdateVideo()
    {
        //1. 비디오 재생
        //2. waitforsecond(0.1f)
        //3. while문 실행
        //4. 비디오 재생 & 조건에 맞춰 비디오 변경
        player.Play();
        yield return new WaitForSeconds(0.1f);
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
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"비디오 출력"))
        {
            SetVideo();
        }

        if (GUI.Button(new Rect(100, 0, 100, 100), "비디오 멈추기"))
        {
            player.Stop();
        }

        if (GUI.Button(new Rect(200, 0, 100, 100), "비디오 반복/해제"))
        {
            player.isLooping = !player.isLooping;
        }
    }
}
