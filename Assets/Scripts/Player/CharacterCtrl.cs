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
        PlaneInput();
        
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        SetMoveDirection();
        CheckGround();
        CheckFrontObject();
        CheckSlope();
        SetGravity();
        SetSlopeMovement();
        SetFrontObjectMovement();
        Debug.Log(isFrontObject);
        rb.AddForce(moveDirection*moveSpeed, ForceMode.VelocityChange);
        rb.AddForce(gravityScale, ForceMode.Force);
        SetMoveRotation();
        LimitSpeed();
    }
    #endregion
}
