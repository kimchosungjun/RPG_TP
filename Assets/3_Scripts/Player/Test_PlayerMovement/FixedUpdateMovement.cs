using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUpdateMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Transform camTransform;
    [SerializeField] float maxStairStep;
    [SerializeField] float moveSpeed;
    [SerializeField] float groundDetectDistance = 0.6f;
    [SerializeField] float detectDistance = 0.6f;
    Vector3 moveDirection;
    Vector3 targetDestination;
    float xMove;
    float zMove;

    int groundLayer = 1 << 3 | 1 << 6;
    RaycastHit groundHit;
    RaycastHit stairHit;

    [SerializeField] bool isCollideGround = false;
    [SerializeField] bool isCollideWall = false;
    [SerializeField] bool isCollideStair = false;

    private void Start()
    {
        if(rb == null) rb= GetComponent<Rigidbody>();
        if (camTransform == null) camTransform = Camera.main.transform; 
        targetDestination = transform.position;
    }

    private void Update()
    {
        InputMoveKey();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        moveDirection = new Vector3(xMove, 0, zMove).normalized;
        //if (moveDirection.magnitude == 0) return;
        moveDirection = camTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;

        isCollideGround = CheckGround();

        isCollideWall = CheckWall();
        isCollideStair = CheckStair();  
        DecideMovement();
        if (Vector3.Distance(targetDestination, transform.position) < 0.05f)
            return;
        rb.MovePosition(Vector3.Lerp(transform.position, targetDestination, Time.fixedDeltaTime * moveSpeed));
    }


    public void InputMoveKey()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");
    }

    public bool CheckWall()
    {
        if (moveDirection == Vector3.zero) return false;
        Vector3 stairVec = Vector3.up * maxStairStep;
        return Physics.CapsuleCast(transform.position + Vector3.up * 2f, transform.position + stairVec * 0.5f, 0.5f, moveDirection, out groundHit, detectDistance, groundLayer);
    }

    

    public bool CheckStair()
    {
        return true;
        //if (isCollideWall == false) return false;
        // 직선거리 = 0.6f (collider의 반지름은 0.5f, 0.1f을 추가해서 벽에 완전히 달라붙지 않도록 설정)
        //float angle = Vector3.Angle(groundHit.normal, moveDirection);
        //if (angle > 90f) angle -= 90f;
        //float stairDistance = detectDistance / Mathf.Cos( angle * Mathf.Deg2Rad);
        //Vector3 origin = transform.position + Vector3.up * maxStairStep * 0.5f;
        //if(Physics.Raycast(origin, moveDirection, out stairHit, detectDistance, groundLayer))
        //{
        //    // 
               
        //}
        //return false;
    }

    public bool CheckGround()
    {
        return Physics.SphereCast(bodyTransform.position, 0.45f, Vector3.down,
            out groundHit, groundDetectDistance, groundLayer);
    }

    public void DecideMovement()
    {
        Vector3 desiredDestination = transform.position + moveDirection * detectDistance;

        // 벽 감지
        if (isCollideWall)
        {
            //Vector3 wallNormal = groundHit.normal;
            //Vector3 slideDirection = Vector3.ProjectOnPlane(moveDirection, wallNormal);
            targetDestination = transform.position /*+ slideDirection * (detectDistance * 0.5f)*/;
        }
        else
        {
            targetDestination = desiredDestination;
        }

        if (Vector3.Distance(targetDestination, transform.position) > detectDistance)
        {
            targetDestination = transform.position + (targetDestination - transform.position).normalized * detectDistance;
        }
    }
}
