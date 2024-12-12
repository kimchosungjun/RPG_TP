/// <summary>
/// PlayerState는 기존 State와 다르게 FixedExecute가 추가했기에
/// 해당 상태를 다루는 PlayerStateMachine을 구현
/// </summary>
public class PlayerStateMachine : StateMachine
{
    public void FixedExecute() { currentState.FixedExecute(); }
}
