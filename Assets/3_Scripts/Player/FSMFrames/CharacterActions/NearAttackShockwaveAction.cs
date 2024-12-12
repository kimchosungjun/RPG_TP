using UnityEngine;

public class NearAttackShockwaveAction : NearAttackAction
{
    public NearAttackShockwaveAction() { }
    public NearAttackShockwaveAction(Collider _attackCollider) : base(_attackCollider) { }

    public override void StopAttack()
    {
        base.StopAttack();
        // Make Shock Wave
    }

    /// <summary>
    /// 콜리더에 이미 접촉한 상태인지 확인 : 이중 피격을 방지하기 위함
    /// True이면 데미지를 줄 수 있다.
    /// </summary>
    /// <param name="_collderName"></param>
    /// <returns></returns>
}
