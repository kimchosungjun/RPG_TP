using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Rigidbody rb = null;
    [SerializeField] Transform bodyTransform = null;
    [SerializeField] Transform mainCamTransform = null;

    /************************** Movement Value *********************************************/

    // Relate Check Place
    [SerializeField] bool isGround = true;
    int ground_wall_Layer = 1 << 3 | 1 << 6;
    public Transform BodyTransform { get { return bodyTransform; } } // Call By MainCam

    // Relate Basic Movement 
    float xInput = 0;
    float zInput = 0;
    Vector3 moveDirection = Vector3.zero;

    // Relate Slope Movement
    RaycastHit slopeHit;
    bool exitSlope = false;
    /*********************************************************************************************/

    [Header("Basic Movement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float limitMoveSpeed = 8f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField, Tooltip("방향키 입력에 따른 플레이어 회전속도")] float rotateSpeed = 5f;
    [SerializeField, Tooltip("땅 까지의 검출 거리")] float groundDist = 1.1f;
    [SerializeField, Tooltip("땅 저항력")] float groundDrag = 2f;
    [SerializeField, Tooltip("공기 저항력")] float airDrag = 0.4f;

    [Header("Slope Movement")]
    [SerializeField, Tooltip("오를 수 있는 최대각도")] float maxSlopeAngle = 60;
    [SerializeField, Tooltip("경사로 검출 거리")] float slopeDist = 1.5f;

    private void Awake()
    {
        if (bodyTransform == null) bodyTransform = this.transform.Find("BodyTranform");
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (mainCamTransform == null) mainCamTransform = Camera.main.transform;
    }

    private void Update()
    {
        GroundCheck();
        InputKey();
        LimitSpeed();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    
    #region Update Method
    private void GroundCheck()
    {
        if (Physics.Raycast(bodyTransform.position, Vector3.down, groundDist, ground_wall_Layer))
        {
            isGround = true;
            rb.drag = groundDrag;
            exitSlope = false;
        }
        else
        {
            isGround = false;
            rb.drag = airDrag;
        }
    }

    private bool SlopeCheck()
    {
        if(Physics.Raycast(bodyTransform.position, Vector3.down, out slopeHit, slopeDist, ground_wall_Layer))
        {
            return ((slopeHit.normal != Vector3.up) && Vector3.Angle(slopeHit.normal, Vector3.up) <= maxSlopeAngle) ? true : false;
        }
        return false;
    }

    void InputKey()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(KeyCode.Space) && isGround) Jump();
    }

    public void Jump()
    {
        exitSlope = true;
        rb.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
    }

    public void LimitSpeed()
    {
        if (rb.velocity.magnitude > limitMoveSpeed)
        {
            Vector3 limitSpeedVector = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized * limitMoveSpeed;
            rb.velocity = new Vector3(limitSpeedVector.x, rb.velocity.y, limitSpeedVector.z);
        }
    }
    #endregion

    #region FixedUpdate Method
    private void MovePlayer()
    {
        moveDirection = new Vector3(xInput, 0, zInput).normalized;
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        if (SlopeCheck() && !exitSlope)
        {
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
            rb.AddForce(slopeMoveDirection * moveSpeed, ForceMode.Impulse);
            // 바운스 효과를 없애기 위해 강제로 땅에 붙인다.
            if(rb.velocity.y>0) rb.AddForce(Vector3.down*80f, ForceMode.Force);
        }
        else
        { 
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Impulse);
        }
        rb.useGravity = !SlopeCheck();
        if (moveDirection.magnitude != 0) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotateSpeed);
    }
    #endregion
}
