using System.Collections.Generic;
using UnityEngine;

public class NearTriggerAttackAction : AttackAction
{
    protected Dictionary<string, Collider> colliderGroup = new Dictionary<string, Collider>();
    [SerializeField] protected Collider coll = null;

    public override void DoAttack()
    {
        if (coll != null)
            coll.gameObject.SetActive(true);
    }
    public override void StopAttack()
    {
        if(coll.gameObject !=null)
            coll.gameObject.SetActive(false);
        colliderGroup.Clear();
    }

    /// <summary>
    /// 콜리더에 이미 접촉한 상태인지 확인 : 이중 피격을 방지하기 위함
    /// True이면 데미지를 줄 수 있다.
    /// </summary>
    /// <param name="_collderName"></param>
    /// <returns></returns>
    public bool CheckCollider(Collider _collision)
    {
        if (colliderGroup.ContainsKey(_collision.name)) return false;
        colliderGroup.Add(_collision.name, _collision);
        return true;
    }
}
