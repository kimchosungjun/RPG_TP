using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionState : PlayerOnGroundState
{
    public PlayerInteractionState(WarriorMovementControl _controller) : base(_controller)  {  }

    // 대화 시엔 아무런 행동 못함
    // 코루틴으로 회전시키고, 애니메이션도 대화 상대를 알아야 할 수 있기에 Interaction은 아무것도 없다.
    // 대화가 끝나고 상태전이는 대화 기능에서 설정해야 함
}
