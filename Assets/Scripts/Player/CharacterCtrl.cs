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
        
        //if (Input.GetKey(KeyCode.Space) && isGround)
        //{
        //    rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        //}
    }

    void FixedUpdate()
    {
        SetMoveDirection();
        CheckGround();
        moveDirection.y = SetGravity();
        moveDirection = SetMovement();
        rb.AddForce(moveDirection, ForceMode.Force);
        LimitSpeed();
    }
    #endregion
}
