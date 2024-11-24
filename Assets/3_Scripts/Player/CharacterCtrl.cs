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
    void Start()
    {
        MovementSetup();
    }

    void Update()
    {
        MovementInput();
    }

    void FixedUpdate()
    {
        SetMoveDirection();
        CheckGround();
        SetSlopeMovement();
        if (moveDirection != Vector3.zero)
            rb.AddForce(moveDirection, ForceMode.Force);
         SetMoveRotation();

        SetGravity();
        if(gravityScale!=Vector3.zero)
            rb.AddForce(gravityScale, ForceMode.Force);
        
        LimitSpeed();
    }
    #endregion
}
