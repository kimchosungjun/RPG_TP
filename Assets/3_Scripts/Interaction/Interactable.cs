using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract string Detect();
    public abstract void Interact();
}


/*

1. 플레이어가 Detect하는 물체를 확인
2. 물체가 있으면 Detect한다.
================================
3. 트리거 밖으로 이동한다.
4. Detect를 취소한다.
    => List나 
================================
3. 키를 눌러 Interact한다.
4. 대화가 가능하면 InteractMode로 이동한다.
5. InteractMode가 되면 플레이어 조작이 불가능 + UI 비활성화
6. 대화가 종료되면 InteractMode를 취소한다.
================================
4. 대화하는 존재가 아니라면 습득, 상호작용하고 사라진다.

 */