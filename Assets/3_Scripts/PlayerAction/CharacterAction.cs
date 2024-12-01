using UnityEngine;

/// <summary>
/// 캐릭터 액션은 공격, 스킬을 사용하기 위해 생성
/// 
/// 플레이어 행동 : 공격하기, 스킬, 궁극기 스킬
/// 몬스터 행동 : 
/// 
/// </summary>
public abstract class CharacterAction 
{
    public abstract void PlayAction();
}


public class BuffAction : CharacterAction
{
    public override void PlayAction()
    {
        
    }
}

public class AttackAction : CharacterAction
{
    public override void PlayAction()
    {
        
    }
}

public class FarAttackAction : AttackAction
{

}

public class NearAttackAction : AttackAction
{

}
