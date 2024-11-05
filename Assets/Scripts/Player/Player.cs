using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // 캐릭터의 회전을 고정하여, 외부 충돌로 넘어지지 않도록 설정
    }

    void Update()
    {
        Move();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
}
