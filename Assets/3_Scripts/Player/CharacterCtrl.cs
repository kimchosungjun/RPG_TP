using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CharacterCtrl : MonoBehaviour
{
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
