using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainCamera : MonoBehaviour
{
    public bool IsCameraAni = false;
    public float ZoomDelta = 0f;

    private void Awake()
    {
        SharedMgr.mainCam = this;
    }

    public class cZoomInfo
    {
        public float StartDelay;
        public bool IsZoom;
        public float Dest;
        public float FadeIn_Velocity;
        public float Duration;
        public float FadeOut_Velocity;

        public float FadeIn_Time;
        public float FadeOut_Time;
        public float FadeIn_MoveSpeed;
        public float FadeOut_MoveSpeed;
        public Vector3 DeltaDir;
    }

    cZoomInfo ZoomInfo = new cZoomInfo();
    //bool IsEndStage = false;
    //bool IsFallowMe = false;

    protected void ResetZoom()
    {
        ZoomDelta = 0f;
        //IsEndStage = false;
        //IsFallowMe = true;
    }

    IEnumerator ZoomEndStageCoroutine()
    {
        CameraFilterPack_Blur_Focus filter = gameObject.AddComponent<CameraFilterPack_Blur_Focus>();
        CameraFilterPack_Blur_Focus.ChangeEyes = 20.0f;

        float dt, delta;

        if(ZoomInfo.StartDelay > 0)
        {
            yield return new WaitForSeconds(ZoomInfo.StartDelay);
        }

        Transform trCon = transform.parent.parent;
        bool MoveCamera = false;

        if(ZoomInfo.DeltaDir != Vector3.zero)
        {
            MoveCamera = true;
            //IsFallowMe = false;
        }

        while(true)
        {
            dt = Time.deltaTime;
            delta = dt * ZoomInfo.FadeIn_Velocity;

            if(CameraFilterPack_Blur_Focus.ChangeEyes > 3.0f)
            {
                CameraFilterPack_Blur_Focus.ChangeEyes -= (17f * dt) / ZoomInfo.FadeIn_Time;

                if (CameraFilterPack_Blur_Focus.ChangeEyes < 3.0f)
                    CameraFilterPack_Blur_Focus.ChangeEyes = 3.0f;
            }

            if(MoveCamera)
            {
                if((ZoomInfo.FadeIn_Time -= dt) > 0f)
                {
                    trCon.position += ZoomInfo.DeltaDir * dt * ZoomInfo.FadeIn_MoveSpeed;
                }
                else
                {
                    trCon.position += ZoomInfo.DeltaDir * (ZoomInfo.FadeIn_Time + dt) * ZoomInfo.FadeIn_MoveSpeed;

                    MoveCamera = false;
                }
            }

            if(ZoomInfo.IsZoom)
            {
                if((ZoomDelta + delta) < ZoomInfo.Dest)
                {
                    ZoomDelta = ZoomInfo.Dest;
                    break;
                }
                ZoomDelta += delta;
            }
            else
            {
                if((ZoomDelta + delta) > ZoomInfo.Dest)
                {
                    ZoomDelta = ZoomInfo.Dest;
                    break;
                }
                ZoomDelta += delta;
            }

            yield return null;
        }

        yield return new WaitForSeconds(ZoomInfo.Duration);

        if (ZoomInfo.DeltaDir != Vector3.zero)
            MoveCamera = true;

        while(true)
        {
            dt = Time.deltaTime;
            delta = dt * ZoomInfo.FadeOut_Velocity;

            if(CameraFilterPack_Blur_Focus.ChangeEyes < 3.0f)
            {
                CameraFilterPack_Blur_Focus.ChangeEyes += (17f * dt) / ZoomInfo.FadeOut_Time;
            }

            if(MoveCamera == true)
            {
                if((ZoomInfo.FadeOut_Time -= dt) > 0f)
                {
                    trCon.position -= ZoomInfo.DeltaDir * dt * ZoomInfo.FadeOut_MoveSpeed;
                }
                else
                {
                    trCon.position -= ZoomInfo.DeltaDir * (ZoomInfo.FadeOut_Time + dt) * ZoomInfo.FadeOut_MoveSpeed;
                    MoveCamera = false;
                }
            }

            if(ZoomInfo.IsZoom)
            {
                if((ZoomDelta + delta) > 0)
                {
                    ZoomDelta = 0;
                    break;
                }
                ZoomDelta += delta;
            }
            else
            {
                if((ZoomDelta + delta) < 0)
                {
                    ZoomDelta = 0;
                    break;
                }
                ZoomDelta += delta;
            }

            yield return null;
        }

        Destroy(filter);
    }

    public void ZoomEndStage(float StartDelay, float ZoomDest, float BlendInTime, float Duration, float BlendOutTime, Vector3 DeltaPos)
    {
        //IsEndStage = true;

        ZoomInfo.FadeIn_Time = BlendInTime * Time.timeScale;
        ZoomInfo.FadeOut_Time = BlendOutTime * Time.timeScale;

        ZoomInfo.StartDelay = StartDelay;
        ZoomInfo.Dest = ZoomDest;
        ZoomInfo.IsZoom = ZoomInfo.Dest < 0;
        ZoomInfo.Duration = Duration * Time.timeScale;
        ZoomInfo.FadeIn_Velocity = ZoomInfo.Dest / ZoomInfo.FadeIn_Time;
        ZoomInfo.FadeOut_Velocity = -ZoomInfo.Dest / ZoomInfo.FadeOut_Time;

        if(DeltaPos != Vector3.zero)
        {
            DeltaPos.z = 0;
            float lenth = DeltaPos.magnitude;
            ZoomInfo.FadeIn_MoveSpeed = lenth / ZoomInfo.FadeIn_Time;
            ZoomInfo.FadeOut_MoveSpeed = lenth / ZoomInfo.FadeOut_Time;
            DeltaPos.Normalize();

            StopCoroutine("ShakeCoroutine");
        }

        ZoomInfo.DeltaDir = DeltaPos;

        ResetZoom();

        StartCoroutine("ZoomEndStageCoroutine");
    }
}
