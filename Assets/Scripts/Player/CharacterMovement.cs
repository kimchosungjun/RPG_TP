using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterCtrl : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] CapsuleCollider coll;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Transform mainCamTransform;

    [Header("이동")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float limitMoveSpeed;
    float moveHorizontal = 0.0f;
    float moveVertical = 0.0f;
    Vector3 moveDirection;
    Quaternion targetRotation;

    [Header("도약")]
    [SerializeField] float jumpForce;
    [SerializeField, Tooltip("공중에서의 이동, 회전 제약"), Range(0.0f, 1f)] float airMultiplier;

    [Header("검출")]
    [SerializeField] bool isGround = false;
    [SerializeField] bool isOnSlope = false;
    [SerializeField] bool isFrontObject = false;
    [SerializeField] bool isOverMaxAngle = false;
    [SerializeField] float maxSlopeAngle = 45f;
    int groundLayer = 1 << 3 | 1 << 6;
    float groundDistance = 0.6f;
    float groundRadius = 0.0f;

    float playerHeight = 1.8f;
    float stepHeight = 0.2f;
    float stepRadius = 0.5f;
   
    RaycastHit groundHit;

    [Header("중력")]
    [SerializeField] float gravityIncrease;
    [SerializeField] float maxGravityScale;
    [SerializeField] float currentGravityScale;
    Vector3 gravityScale;
    /// <summary>
    /// Call By Start : 초기화 할 변수 모음
    /// </summary>
    public void MovementSetup()
    {
        if (mainCamTransform == null) mainCamTransform = Camera.main.transform;
        groundRadius = coll.radius * 0.9f;
        targetRotation = transform.rotation;
    }



    #region Plane
    public void PlaneInput()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");
    }

    public void SetMoveDirection() 
    {
        moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        moveDirection = mainCamTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;
    }

    public void SetMoveRotation()
    {
        Quaternion curRotation = transform.rotation;
        if (moveDirection == Vector3.zero)
        {
            if(Quaternion.Angle(targetRotation, curRotation)<3f)
                return;
            transform.rotation = Quaternion.Slerp(curRotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        }
        else
        {
            Vector3 lookDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
            lookDirection = mainCamTransform.TransformDirection(lookDirection);
            lookDirection.y = 0f;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            targetRotation = lookRotation;
            transform.rotation = Quaternion.Slerp(curRotation, lookRotation, Time.fixedDeltaTime * rotateSpeed);
        }
    }
    #endregion

    #region Check
    public void CheckGround()
    {
        isGround = Physics.SphereCast(bodyTransform.position, groundRadius, Vector3.down, 
            out groundHit, groundDistance, groundLayer);
    }

    public void CheckFrontObject()
    {
        isFrontObject = Physics.CapsuleCast(transform.position + Vector3.up * playerHeight, transform.position + Vector3.up * stepHeight, stepRadius, moveDirection, 0.1f, groundLayer);
    }

    public void CheckSlope()
    {
        if (!isGround)
        {
            isOnSlope = false;
            isOverMaxAngle = false;
            return;
        }
        
        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        if (angle <= maxSlopeAngle)
        {
            if (angle == 0.0f) isOnSlope = false;
            else isOnSlope = true;
            isOverMaxAngle = false;
        }
        else
        {
            isOnSlope = true;
            isOverMaxAngle = true;
        }
    }
    #endregion

    #region Control Force
    public void LimitSpeed()
    {
        if (isOnSlope)
        {
            if (isOverMaxAngle == false)
            {
                if (rb.velocity.magnitude > limitMoveSpeed)
                {
                    rb.velocity = rb.velocity.normalized * limitMoveSpeed;
                    return;
                }
            }
        }

        Vector3 velocityVec = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (velocityVec.magnitude > limitMoveSpeed)
        {
            velocityVec = velocityVec.normalized;
            rb.velocity = new Vector3(velocityVec.x * limitMoveSpeed, rb.velocity.y, velocityVec.z * limitMoveSpeed);
        }
    }

    public void SetGravity()
    {
        if(isGround)
        {
            if (isOverMaxAngle)
            {
                currentGravityScale += Time.fixedDeltaTime * gravityIncrease * rb.mass;
                if (currentGravityScale < maxGravityScale)
                    currentGravityScale = maxGravityScale;
            }
            else currentGravityScale = 0f;
        }
        else
        {
            currentGravityScale +=Time.fixedDeltaTime * gravityIncrease * rb.mass;
            if (currentGravityScale < maxGravityScale)
                currentGravityScale = maxGravityScale;
        }

        gravityScale = Vector3.up * currentGravityScale;
    }

    public void SetFrontObjectMovement()
    {
        if (isFrontObject)
        {
            moveDirection = Vector3.zero;
        }
    }

    public void SetSlopeMovement()
    {
        if (isOnSlope)
        {
            moveDirection = Vector3.ProjectOnPlane(moveDirection, groundHit.normal).normalized;
            if (isOverMaxAngle)
            {
                float angle = Vector3.Angle(Vector3.up, groundHit.normal);
                float ratio = angle - maxSlopeAngle;
                ratio = Mathf.Clamp(ratio, 0f, 10f);
                moveDirection *= (10f-ratio) / 10;
            }
        }
    }
    #endregion
}
