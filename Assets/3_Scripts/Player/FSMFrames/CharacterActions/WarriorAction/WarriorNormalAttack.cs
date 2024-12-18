using UnityEngine;

public class WarriorNormalAttack : NearAttackAction
{
    [SerializeField, Tooltip("Int형 레이어")] int enemyLayer = 7;
    PlayerStat playerStat = null;
    PlayerNormalAttackActionSOData soData = null;
    TransferAttackData attackData = new TransferAttackData();
    int combo;

    // DoAction이 작동하면 각자 SO에서 계수와 효과를 불러온다.
    // 현재 스탯에 접근하여 스탯을 불러온다.
    // 계수와 스탯을 조합하여 데이터를 전달한다.
    // DoAction에서 공격을 활성화 or 생성한다.
    // MovementCtrl에서 해당 Action을 관리하는게 좋을듯하다.
    // 애니메이션에서 설정을 관리하는건 아닌거같음. 그냥 활성화만 하는게 나을듯.

    public void SetStat(PlayerStat _playerStat, int _combo, PlayerNormalAttackActionSOData _soData) 
    {
        playerStat = _playerStat; 
        combo = _combo;
        soData = _soData;
    }

    public override void DoAction()
    {
        float damageValue = soData.GetActionMultiplier(combo) * playerStat.Attack * Randoms.GetCritical(playerStat.Critical);
        attackData.SetData(soData.GetAttackEffectType(combo),damageValue, soData.GetMaintainTime(combo));
        base.DoAction(); // Do Attack
    }
    

    public override void StopAttack()
    {
        base.StopAttack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == enemyLayer)
        {
            // 처음 닿는 물체라면 List에 추가해주고 데미지 
            if (CheckCollider(other))
                other.GetComponent<BaseActor>().TakeDamage(attackData);
        }
    }
}
