using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    protected Dictionary<string, Collider> collideGroup = new Dictionary<string, Collider>();   
    [SerializeField] SphereCollider coll;
    int enemyLayer = -1;
    float destRadius = -1f;
    
    // To Do ~~~~~~ 데미지 + 이펙트
    //float damageValue = -1f;

    private void Awake()
    {
        StartCoroutine(Cor());
    }

    IEnumerator Cor()
    {
        Vector3 startVec = transform.localScale;
        Vector3 destVec = new Vector3(15, 15, 15);
        float time = 0f;
        while (true)
        {
            if (time > 3f)
                break;
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startVec, destVec, time/2f);
            yield return null;  
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// 시작, 끝 반지름, 생성위치, 적 레이어, 데미지, 이펙트
    /// </summary>
    /// <param name="_startRadius"></param>
    /// <param name="_destRadius"></param>
    /// <param name="_setPostion"></param>
    /// <param name="_enemyLayer"></param>
    public void SetupShockwave(float _startRadius, float _destRadius, Vector3 _setPostion, UtilEnums.LAYERS _enemyLayer)
    {
        coll.radius = _startRadius;
        destRadius = _destRadius;
        transform.position = _setPostion;
        enemyLayer = (int)_enemyLayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
            if (!collideGroup.ContainsKey(other.name))
            {
                collideGroup.Add(other.name, other);
                // To Do ~~~~~
                // 데미지
            }
        }
    }
}
