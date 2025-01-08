using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CowardMonster : NonCombatMonster
{
    [SerializeField] protected Transform player;

    [SerializeField] protected bool isHitState = false; // 맞을때만 활성화
    protected bool isMoving = false; // 도망칠때만 활성화
    protected float maxDistance = 5f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected bool isIdleMove = false;
    [SerializeField] protected StatusUI statusUIController;

    protected override void Awake()
    {
        if(anim==null)  anim = GetComponent<Animator>();    
        if(statusUIController==null) statusUIController = GetComponentInChildren<StatusUI>();    
    }

    protected virtual void Update()
    {
      //  statusUIController.FixedExecute();
    }
}
