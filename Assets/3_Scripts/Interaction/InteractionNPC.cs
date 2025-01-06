using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionNPC : Interactable
{
    // 대사 로드 만들기
    // 그 이후 구현하기

    public override string Detect()
    {
        return "테스트용 NPC 입니다.";
    }

    public override void Interact()
    {
        Debug.Log("성공적인 상호작용 입니다.");
        SharedMgr.InteractionMgr.RemoveInteractable(this);  
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);   
    }
}
