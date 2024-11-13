using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform mainCamTransfrom = null;
    [SerializeField] Transform bodyTransform = null;
    [SerializeField] Rigidbody rigid = null;

    [Header("Slope")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;
    bool existSlope;

    [Header("Dash")]
    [SerializeField] float dashForce = 20f;
    [SerializeField] float dashCoolDownTime = 3f;
    [SerializeField] float dashTime = 0.25f;
    [SerializeField] bool isDashTime = false;
    [SerializeField] bool isDoDash = false;

    [Header("Jump")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpCoolDownTime = 0.25f;
    bool isDoJump = false;

    [Header("Ground Check")]
    [SerializeField] float playerHeight = 1f;
    int groundLayer = 1 << 3 | 1 << 6;
    bool isGround = false;
    float groundDrag = 5f;

    [Header("Movement")]
    [SerializeField] float moveForce;
    [SerializeField] float rotationForce;
    [SerializeField] float groundMoveForce;
    [SerializeField] float groundRotationForce;
    [SerializeField] float airMoveForce;
    [SerializeField] float airRotationForce;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    private void Awake()
    {
        if (rigid == null) rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;
    }

    private void Start()
    {
        if (mainCamTransfrom == null) mainCamTransfrom = Camera.main.transform;
    }

    private void Update()
    {
        GroundCheck();
        PlaneMovementInput();
        SprintInput();
        JumpInput();
        LimitPlaneMovementSpeed();
    }

    public void GroundCheck()
    {
        if(Physics.Raycast(bodyTransform.position, Vector3.down, playerHeight + 0.2f, groundLayer) != isGround)
        {
            isGround = !isGround;
            if (isGround)
            {
                rigid.drag = groundDrag;
                moveForce = groundMoveForce;
                rotationForce = groundRotationForce;
            }
            else
            {
                rigid.drag = 0f;
                moveForce = airMoveForce;
                rotationForce = airRotationForce;
            }
        }   
    }

    public void PlaneMovementInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void SprintInput()
    {
        if (!isDoDash && isGround && Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log(transform.forward);
            rigid.velocity = transform.forward * dashForce;
            isDoDash = true;
            isDashTime = true;
            Invoke(nameof(EndDashTime), dashTime);
            Invoke(nameof(EndDoDash), dashCoolDownTime);
        }
    }
    public void EndDashTime() { isDashTime = false; }
    public void EndDoDash() { isDoDash = false; }

    public void JumpInput()
    {
        if (isGround && !isDoJump && Input.GetKeyDown(KeyCode.Space))
        {
            existSlope = true;
            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            isDoJump = true;
            Invoke(nameof(EndDoJump), jumpCoolDownTime);
        }
    }

    public void EndDoJump() { isDoJump = false;  existSlope = false; }
    public void LimitPlaneMovementSpeed()
    {
        if (isDashTime)
            return;

        if (OnSlope() && !existSlope)
        {
            if (rigid.velocity.magnitude > moveForce)
                rigid.velocity = rigid.velocity.normalized * moveForce;
        }
        else
        {
            Vector3 velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            if (velocity.magnitude > groundMoveForce)
            {
                Vector3 limitVelocity = velocity.normalized * groundMoveForce;
                rigid.velocity = new Vector3(limitVelocity.x, rigid.velocity.y, limitVelocity.z);
            }
        }
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(bodyTransform.position, Vector3.down, out slopeHit, playerHeight + 1f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle != 0 && angle < maxSlopeAngle;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection() { return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized; }

    private void FixedUpdate()
    {
        PlaneMovement();
    }

    public void PlaneMovement()
    {
        // Set Move Direction : Consider Camera Look Direction
        if (isDashTime)
            return;
        
        moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        moveDirection = mainCamTransfrom.TransformDirection(moveDirection);
        moveDirection.y = 0;

        if (OnSlope() && !existSlope)
        {
            rigid.AddForce(GetSlopeMoveDirection() * moveForce, ForceMode.Force);
            if (rigid.velocity.y > 0)
                rigid.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else
        {
            rigid.AddForce(moveDirection * moveForce, ForceMode.Force);
        }
        if(moveDirection!=Vector3.zero)
            rigid.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.fixedDeltaTime * rotationForce);
        rigid.useGravity = !OnSlope();
    }
}


