using System.Collections;
using UnityEngine;

public class CharacterCtrl : MonoBehaviour
{
    bool canChangeCharacter = true; // 처음 시작 시 초기화 필요
    float changeCharacterCoolTime = 1f; 

    int currentCharacter = 0; // 처음 시작 시 초기화 필요
    [SerializeField] CharacterMovement[] characterMovements;

    private void Awake()
    {
        characterMovements[0].Init();
    }

    private void Start()
    {
        characterMovements[0].Setup();
    }

    private void Update()
    {
        characterMovements[0].Execute();
    }

    private void FixedUpdate()
    {
        characterMovements[0].FixedExecute();
    }

    public void ChangeCharacter(int _index)
    {
        // 현재 캐릭터와 같으면 작동 안함
        if (currentCharacter == _index) return;

        // 쿨타임인지 확인 : 쿨타임이 아니라면 canChangeCharacter는 true
        if (canChangeCharacter == false) return;

        // To Do ~~~~~~
        // 캐릭터가 활성화 할 수 있는 상태인지 확인

        // To Do ~~~~~~ 
        // 앞의 조건에 해당되지 않는다면 바꾼다.
        StartCoroutine(CMeasureChangeCoolTime());
    }

    /// <summary>
    /// 1초간 바꿀 수 없도록 조건 변경
    /// </summary>
    /// <returns></returns>
    IEnumerator CMeasureChangeCoolTime()
    {
        canChangeCharacter = false;
        float time = 0f;
        while(time< changeCharacterCoolTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        canChangeCharacter = true;
    }
}
