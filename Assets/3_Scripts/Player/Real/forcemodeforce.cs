using System.Collections;
using UnityEngine;

public class forcemodeforce : MonoBehaviour
{
    #region Component
    
    [Header("컴포넌트 : 연결해주기")]
    [SerializeField] Transform camTransform;
    [SerializeField] Transform bodyTransform;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Animator anim;
    [SerializeField] CapsuleCollider coll;
    #endregion

    #region Movement

    /******************************************/
    /************** 평면 이동 ****************/
    /******************************************/
    [Header("이동")]
    [SerializeField] float playerMoveSpeed;
    [SerializeField] float playerMoveLimitSpeed;
    [SerializeField] float playerWalkSpeed;
    [SerializeField] float playerRotateSpeed;
    [SerializeField] float planeCoefficient = 10f;
    [SerializeField] float slopeCoefficient = 20f;
    float xMove;
    float zMove;
    Vector3 moveDirection;
    Quaternion moveRotation;

    /******************************************/
    /***************** 점프  ******************/
    /******************************************/
    [SerializeField] float playerJumpForce;
    [SerializeField, Range(0,1f)] float airMovementMultiplier;

    /******************************************/
    /***************** 대쉬  ******************/
    /******************************************/
    [SerializeField] float playerSprintSpeed;
    [SerializeField] float sprintMaintainTimer = 0.15f;
    [SerializeField] float sprintCoolTimer = 0.3f;
    bool isPlayerSprint = false;
    bool canSprint = true;
    #endregion

    #region Detect

    /******************************************/
    /*********** 공통 검출 변수 *************/
    /******************************************/
    float playerBodyRadius;

    /******************************************/
    /**************** 땅 검출 ****************/
    /******************************************/
    [Header("검출")]
    [SerializeField, Tooltip("땅 검출 시, 추가 검출 거리")] float detectGroundDelta;
    [SerializeField, Tooltip("땅 저항 값")] float groundDrag;
    [SerializeField] bool isOnGround = false;
    [SerializeField] float stepHeight = 0.2f;
    [SerializeField] float groundDetectDistance;

    float slopeDetectDistance;
    int groundLayer = 1 << 3 | 1 << 6; // 3은 벽, 6은 땅
    RaycastHit groundHit;


    [SerializeField] float slopeMaxAngle = 50f;

    /******************************************/
    /**************** 중력   ******************/
    /******************************************/
    [SerializeField] float gravityIncreasemenet;
    [SerializeField] float curGravity;
    [SerializeField] float minGravity;
    [SerializeField] float maxGravity;
    #endregion

    #region Gravity

    #endregion

    void Awake()
    {
        if(rigid == null) rigid =GetComponentInChildren<Rigidbody>();    
        if(anim == null) anim = GetComponentInChildren<Animator>();
        if(coll==null) coll = GetComponentInChildren<CapsuleCollider>();
    }

    void Start()
    {
        if (camTransform == null) camTransform = Camera.main.transform;
        if(coll !=null) playerBodyRadius = coll.radius;
        moveDirection = Vector3.zero;
        moveRotation = transform.rotation;
        playerMoveSpeed = playerWalkSpeed;
        groundDetectDistance = bodyTransform.position.y - playerBodyRadius + detectGroundDelta;
        slopeDetectDistance = stepHeight * 0.5f * 3.5f + playerBodyRadius;
    }

    void Update()
    {
        GroundCheck();
        InputMovementKey();
        LimitMovementSpeed();
    }

    public void GroundCheck()
    {
        isOnGround = Physics.SphereCast(bodyTransform.position, playerBodyRadius, Vector3.down, out groundHit, groundDetectDistance, groundLayer);
        if (isOnGround)
            rigid.drag = groundDrag;
        else
            rigid.drag = 0f;
    }

    public void InputMovementKey()
    {
        xMove = Input.GetAxisRaw("Horizontal");
        zMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isOnGround) { Jump(); }
        if (Input.GetMouseButtonDown(1)) { Sprint(); }
    }

    public void Jump()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        rigid.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    }

    public void Sprint() 
    { 
        if (canSprint) 
        { 
            canSprint = false;
            playerMoveSpeed = playerSprintSpeed;
            StartCoroutine(CSprintCoolState()); 
        } 
    }
    IEnumerator CSprintCoolState()
    {
        float time = 0f;
        bool maintain = true;
        while (true)
        {
            time+= Time.deltaTime;
            if(time>sprintMaintainTimer && maintain)
            {
                maintain = false;
                isPlayerSprint = false;
                playerMoveSpeed = playerWalkSpeed;
            }

            if(time > sprintCoolTimer)
            {
                canSprint = true;
                yield break;
            }
            yield return null;
        }
    }
   
    public void LimitMovementSpeed()
    {
        if (isPlayerSprint) return;

        Vector3 planeVelocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        if (planeVelocity.magnitude > playerMoveLimitSpeed)
        {
            planeVelocity = planeVelocity.normalized;
            planeVelocity *= playerMoveLimitSpeed;
            planeVelocity.y = rigid.velocity.y;
            rigid.velocity = planeVelocity;
        }
    }



    /******************************************/
    /************** 물리 적용 ***************/
    /******************************************/

    void FixedUpdate()
    {
        SetGravity();
        Movement();
        ApplyMovementForce(); 
        ApplyMovementRotation();
    }

    public void Movement()
    {
        moveDirection = new Vector3(xMove, 0f, zMove);

        moveDirection = camTransform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;
    }

    public void SetGravity()
    {
        if (isOnGround)
        {
            curGravity = minGravity;
        }
        else
        {
            curGravity += gravityIncreasemenet * Time.fixedDeltaTime;
            if (curGravity < maxGravity) curGravity = maxGravity;
        }
    }

    public void SlopeMovement()
    {
        if (isOnGround)
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);

            if (angle < 0.1f)
                return;

            if (angle <= slopeMaxAngle)
            {
                Vector3 direction = Vector3.ProjectOnPlane(moveDirection, groundHit.normal);
                moveDirection = direction.normalized;
            }
            //else
            //{
            //    Vector3 origin = transform.position + Vector3.up * stepHeight * 0.5f;
            //    if(Physics.Raycast(origin, moveDirection, slopeDetectDistance ,groundLayer))
            //    {

            //    }
            //}
        }
    }

    public void ApplyMovementForce()
    {
        if(moveDirection != Vector3.zero)
            moveRotation = Quaternion.LookRotation(moveDirection);
        
        if (isOnGround)
        {
            rigid.AddForce(moveDirection * playerMoveSpeed * planeCoefficient, ForceMode.Force);
            return;
        }

        Vector3 airMovement = new Vector3(moveDirection.x, planeCoefficient, moveDirection.z);
        airMovement *= airMovementMultiplier * playerMoveSpeed * planeCoefficient;
        airMovement.y = curGravity * rigid.mass;
        rigid.AddForce(airMovement, ForceMode.Force);
    }

   
    public void ApplyMovementRotation()
    {
        if (Quaternion.Angle(transform.rotation, moveRotation) > 1f)
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, Time.fixedDeltaTime * playerRotateSpeed);
    }
}
