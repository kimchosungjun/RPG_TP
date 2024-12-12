using System.Collections.Generic;
using UnityEngine;

public class NearAttackAction : AttackAction
{
    protected Dictionary<string, Collider> colliderGroup = new Dictionary<string, Collider>();
    protected Collider collider = null;

    public NearAttackAction() { }
    public NearAttackAction(Collider _attackCollider) 
    { 
        this.collider = _attackCollider;  
    }   

    public override void DoAttack()
    {
        if (collider != null)
            collider.gameObject.SetActive(true);
    }

    public override void StopAttack()
    {
        if(collider.gameObject !=null)
            collider.gameObject.SetActive(false);
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
