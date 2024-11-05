using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    public float moveSpeed = 5f;        // 이동 속도
    public float jumpForce = 5f;        // 점프 시 힘
    public float gravityMultiplier = 2f; // 중력 가속도 조절

    private Rigidbody rb;
    private bool isGrounded = true;     // 바닥에 닿아 있는지 여부 확인

  

    void Move()
    {
        // 입력을 받음
        float horizontalInput = Input.GetAxis("Horizontal"); // A, D 키 또는 좌우 방향키
        float verticalInput = Input.GetAxis("Vertical");     // W, S 키 또는 상하 방향키

        // 입력에 따른 이동 방향 계산
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 이동 벡터를 카메라 방향에 맞춰 회전
        moveDirection = Camera.main.transform.TransformDirection(moveDirection);
        moveDirection.y = 0; // 수직 이동을 방지하여 캐릭터가 바닥에 평행하게 이동하도록 함

        // 이동 속도 적용
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void Jump()
    {
        // 점프 힘을 위쪽 방향으로 추가
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        isGrounded = false; // 점프 중에는 바닥에 닿아 있지 않음
    }

    private void FixedUpdate()
    {
        // 추가적인 중력 적용 (더 빠른 낙하 속도를 위해 중력 증가)
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * gravityMultiplier * Physics.gravity.y * rb.mass);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 바닥과 충돌했을 때만 isGrounded를 true로 설정
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
