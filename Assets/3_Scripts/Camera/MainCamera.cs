using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    bool isCameraShake = false;
    Transform shakeTransform;

    public class CameraShakeInfo
    {
        public float startDelay;
        public bool useTotalTime;
        public float totalTime;
        public Vector3 destination;
        public Vector3 shake;
        public Vector3 direction;

        public float remainDistance;
        public float remainCountDis;
        public bool isUseCount;
        public int count;
        public float velocity;
        public bool useDamping;
        public float damping;
        public float dampingTime;
    }

    CameraShakeInfo shakeInfo = new CameraShakeInfo();
    Vector3 originalPosition;
    // 보정 값
    // Fov : Field Of View (물체를 볼 수 있는 영역)
    float FovX = 0.2f;
    float FovY = 0.2f;
    float Left = 1.0f;
    float Right = -1.0f;

    #region Life Cycle
    private void Awake()
    {
        SharedMgr.MainCamera = this;
        originalPosition = transform.position;

        InitShake();
    }

    private void InitShake()
    {
        shakeTransform = transform.parent;
        isCameraShake = false;
    }

    private void ResetShakeTransform()
    {
        shakeTransform.localPosition = Vector3.zero;
        isCameraShake = false;
        CameraLimit();
    }

    private void CameraLimit(bool _isOriginalPositionY = false)
    {
        Vector3 cameraPos = originalPosition;
        if (cameraPos.x - FovX < Left)
            cameraPos.x = Left + FovX;
        else if(cameraPos.x + FovX > Right)
            cameraPos.x = Right - FovX;

        if (_isOriginalPositionY)
            cameraPos.y = originalPosition.y;
    }

    public void Shake(int _cameraID)
    {
        shakeInfo.startDelay = 0f;
        shakeInfo.totalTime = 3f;
        shakeInfo.useTotalTime = true;

        shakeInfo.shake = new Vector3(0.2f,0.2f,0f);

        shakeInfo.destination = shakeInfo.shake;
        shakeInfo.direction = shakeInfo.shake;
        shakeInfo.direction.Normalize();

        shakeInfo.remainDistance = shakeInfo.shake.magnitude;
        shakeInfo.remainCountDis = float.MaxValue; // 중간에 회전하다 멈추는 것을 보정해주는 변수

        shakeInfo.velocity = 8;
        shakeInfo.damping = 0.5f;
        shakeInfo.useDamping = true;
        shakeInfo.dampingTime = shakeInfo.remainDistance / shakeInfo.velocity;

        shakeInfo.count = 4;
        shakeInfo.isUseCount = true;

        StopCoroutine("CShake");
        ResetShakeTransform();
        StartCoroutine("CShake");
    }

    IEnumerator CShake()
    {
        isCameraShake = true;

        float dt, dist;
        if(shakeInfo.startDelay > 0)
            yield return new WaitForSeconds(shakeInfo.startDelay);
        
        while(true)
        {
            dt = Time.fixedDeltaTime;
            dist = dt * shakeInfo.velocity;
            if ((shakeInfo.remainDistance -= dist) > 0)
            {
                shakeTransform.localPosition += shakeInfo.direction * dist;
                float rc = transform.position.x - FovX - Left;

                if (rc < 0)
                    shakeTransform.localPosition += new Vector3(-rc, 0, 0);
                rc = Right - (transform.position.x + FovX);

                if (rc < 0)
                    shakeTransform.localPosition += new Vector3(rc, 0, 0);

                CameraLimit(true);

                if (shakeInfo.isUseCount)
                {
                    if ((shakeInfo.remainCountDis -= dist) < 0)
                    {
                        shakeInfo.remainCountDis = float.MaxValue;

                        if (--shakeInfo.count < 0)
                            break;
                    }
                }
            }
            else
            {
                if (shakeInfo.useDamping)
                {
                    float distDamping = Mathf.Max(shakeInfo.damping * shakeInfo.dampingTime, shakeInfo.damping * dt);
                    if (shakeInfo.shake.magnitude > distDamping)
                        shakeInfo.shake -= shakeInfo.direction * distDamping;
                    else
                    {
                        shakeInfo.isUseCount = true;
                        shakeInfo.count = 1;
                    }
                }

                shakeTransform.localPosition = shakeInfo.destination - shakeInfo.direction * (-shakeInfo.remainDistance);
                float rc = transform.position.x - FovX - Left;

                if (rc < 0)
                    shakeTransform.localPosition += new Vector3(-rc, 0, 0);

                rc = Right - (transform.position.x + FovX);

                if (rc < 0)
                    shakeTransform.localPosition += new Vector3(rc, 0, 0);

                CameraLimit(true);

                shakeInfo.shake = -shakeInfo.shake;
                shakeInfo.destination = shakeInfo.shake;
                shakeInfo.direction = -shakeInfo.direction;

                float len = shakeInfo.shake.magnitude;
                shakeInfo.remainCountDis = len + shakeInfo.remainDistance;
                shakeInfo.remainDistance += len * 2f;
                shakeInfo.dampingTime = shakeInfo.remainDistance / shakeInfo.velocity;

                if (shakeInfo.remainDistance < dist)
                    break;
            }

            if (shakeInfo.useTotalTime && (shakeInfo.totalTime -= dt) < 0)
                break;

            yield return new WaitForFixedUpdate();
        }

        ResetShakeTransform();
        yield break;
    }
    #endregion
}
