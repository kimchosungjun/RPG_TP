using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMovement : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float baseOrbitRadius = 3f; // 기본 반지름
    public float orbitSpeed = 50f; // 도는 속도 (각도 단위)
    public float noiseAmplitude = 1f; // 반지름에 추가할 노이즈 크기
    public float noiseFrequency = 0.5f; // 노이즈 변화 속도
    [SerializeField]private NavMeshAgent agent;
    private float currentAngle = 0f; // 현재 각도
    [SerializeField] Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null)
        {
            Debug.LogError("플레이어를 설정하세요!");
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // 1. 각도 업데이트
            currentAngle += orbitSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            // 2. 노이즈 추가된 반지름 계산
            float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, 0f) * noiseAmplitude;
            float currentRadius = baseOrbitRadius + noise;

            // 3. 새로운 목표 위치 계산 (플레이어 기준)
            Vector3 offset = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad) * currentRadius,
                0f,
                Mathf.Sin(currentAngle * Mathf.Deg2Rad) * currentRadius
            );
            Vector3 targetPosition = player.position + offset;

            // 4. NavMeshAgent 목적지 설정
            Vector3 forward = transform.forward;
            Vector3 direction = targetPosition - transform.position;
            Vector3 cross = Vector3.Cross(forward, direction);
            if(cross.y < 0)
            {
                anim.SetFloat("Dir", -1f);
                // 오른쪽
            }
            else if(cross.y > 0)
            {
                anim.SetFloat("Dir", 1f);
                //왼쪽
            }
            else
            {
                anim.SetFloat("Dir", 0f);
            }
            agent.SetDestination(targetPosition);

            // 5. 몬스터가 플레이어를 바라보도록 설정 (옵션)
            transform.LookAt(player);
        }
    }
}
