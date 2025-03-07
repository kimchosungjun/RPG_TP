using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스택 : 되돌리기 or ui 창 닫기 순서
// 큐 : 키 입력 or 서버 데이터 처리

public class AIBase 
{
    protected BaseCharacter character;
    protected EAI state = EAI.EAI_CREATE;

    public void Init(BaseCharacter _character)
    {
        character = _character;
    }

    public void State()
    {
        switch (state)
        {
            case EAI.EAI_CREATE: Create(); break;
            case EAI.EAI_SEARCH: Search(); break;
            case EAI.EAI_MOVE: Move(); break;
            case EAI.EAI_RESET: Reset(); break;

        }
    }

    protected virtual void Create()
    {
        state = EAI.EAI_SEARCH;
    }

    protected virtual void Search()
    {
        state = EAI.EAI_MOVE;
    }

    protected virtual void Move()
    {
        state = EAI.EAI_SEARCH;
    }

    protected virtual void Reset()
    {
        state = EAI.EAI_SEARCH;
    }

}
public enum EAI
{
    EAI_CREATE,
    EAI_SEARCH,
    EAI_MOVE,
    EAI_RESET
}