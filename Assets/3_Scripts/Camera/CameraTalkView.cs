using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CameraTalkView : MonoBehaviour
{
    Transform camTransform = null; // 카메라의 Transform
    Vector2 offset = Vector2.zero;   // 캐릭터로부터 얼마나 떨어져 있는지에 대한 변위(위치)
    [SerializeField] float moveTalkViewTime;

    Vector3 legacyPostion;
    Quaternion legacyRotate;

    public void Setup()
    {
        offset = new Vector3(2, 2);
        camTransform = SharedMgr.GameCtrlMgr.GetCameraCtrl?.transform;
    }

    public void Talk(Transform _newTarget) 
    {
        SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetIsLockPlayerContro = true;
        legacyPostion = camTransform.position;
        legacyRotate = camTransform.rotation;   
        StartCoroutine(CMoveToTalk(_newTarget));
    }

    public void EndTalk(UnityAction _endTalkAction)
    {
        StopAllCoroutines();
        StartCoroutine(CRestCamera(_endTalkAction));
    }

    IEnumerator CMoveToTalk(Transform _newTarget)
    {
        Transform playerTrasnfrom = SharedMgr.GameCtrlMgr.GetPlayerCtrl.GetPlayer.transform;

        Vector3 halfPoint = (_newTarget.position + playerTrasnfrom.position) / 2;
        Vector3 lookPosition = halfPoint;
        Vector3 offSetCalDirection = _newTarget.position - lookPosition;
        offSetCalDirection.y = 0;
        offSetCalDirection = offSetCalDirection.normalized; 
        Vector3 offSetDirection = Vector3.Cross(offSetCalDirection, Vector3.down);

        Vector3 camDirection = camTransform.forward;
        camDirection.y= 0;
        camDirection = camDirection.normalized;

        float dot = Vector3.Dot(camDirection, offSetDirection);
        if (dot > 0f)
            offset.x *= -1; 
        lookPosition = offSetDirection * offset.x + Vector3.up * offset.y;

        Vector3 lookDirection =   halfPoint + Vector3.up - lookPosition;
        //lookDirection = lookDirection.normalized;
        
        Vector3 startPosition = camTransform.position;
        Quaternion startRotation = camTransform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(lookDirection);
        float time = 0f;
        while (true)
        {
            time += Time.fixedDeltaTime;
            if (time > moveTalkViewTime) break;
            camTransform.position = Vector3.Slerp(startPosition, lookPosition, time / moveTalkViewTime); 
            camTransform.rotation = Quaternion.Slerp(startRotation, endRotation, time / moveTalkViewTime); 
            yield return new WaitForFixedUpdate();
        }
        camTransform.position = lookPosition;
        camTransform.rotation = endRotation;
    }

    IEnumerator CRestCamera(UnityAction _endTalkAction)
    {
        float time = 0f;
        Vector3 startPosition = camTransform.position;
        Quaternion startRotate = camTransform.rotation; 
        while (true)
        {
            time += Time.fixedDeltaTime;
            if (time > 1f)
                break;

            camTransform.position = Vector3.Slerp(startPosition, legacyPostion, time / 1f);
            camTransform.rotation = Quaternion.Slerp(startRotate, legacyRotate, time / 1f);
            yield return new WaitForFixedUpdate();
        }

        SharedMgr.GameCtrlMgr.GetPlayerCtrl.SetIsLockPlayerContro = false;
        if (_endTalkAction == null)
            yield break;
        _endTalkAction.Invoke();
    }
}
