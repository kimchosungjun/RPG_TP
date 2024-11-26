using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ai 처리는 fixedupdate와 update가 좋다
// 이유는 동기화 처리때문에

public partial class CharacterCtrl : MonoBehaviour
{
    //StateMachine stateMachine = new StateMachine();
    
    PlayerAttackCombo attackCombo = new PlayerAttackCombo();

    #region Unity Life Cycle
    void Awake()
    {
        if (rigid == null) rigid = GetComponentInChildren<Rigidbody>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (coll == null) coll = GetComponentInChildren<CapsuleCollider>();
    }

    void Start()
    {
        if (camTransform == null) camTransform = Camera.main.transform;
        if (coll != null) playerBodyRadius = coll.radius;
        moveDirection = Vector3.zero;
        moveRotation = transform.rotation;
        playerMoveSpeed = playerWalkSpeed;
        groundDetectDistance = bodyTransform.position.y - playerBodyRadius + detectGroundDelta;
        slopeDetectDistance = stepHeight * 0.5f * 5f + playerBodyRadius;
        playerBodyHeight = coll.height;
    }

    void Update()
    {
        GroundCheck();
        InputMovementKey();
        LimitMovementSpeed();
        UpdateAnimation();
        // 공격
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetInteger("AttackCombo", tempCombo);
            anim.SetTrigger("Attack");
            tempCombo += 1;
            if (tempCombo >= 3) tempCombo = 0;
        }

        anim.SetFloat("VerticalVelocity", rigid.velocity.y);

        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("Hit");
        }
    }

    void FixedUpdate()
    {
        Movement();
        SlopeMovement();
        AirBlock();
        SetGravity();
        SetRotation();
        ApplyMovementForce();
        ApplyMovementRotation();
    }
    #endregion
}
