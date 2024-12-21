using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCtrl : MonoBehaviour
{
    /**********************************************/
    /************ 캐릭터 변경 변수 *************/
    /**********************************************/

    #region Value
    [Header("동작하는 캐릭터 종류"), SerializeField] List<BasePlayer> players;
    bool canChangeCharacter = true; // 처음 시작 시 초기화 필요
    float changeCharacterCoolTime = 1f; 
    int currentCharacter = 1; // 처음 시작 시 초기화 필요
    #endregion

    #region Life Cycle
    private void Awake()
    {
        players[currentCharacter].Init();
    }

    private void Start()
    {
        players[currentCharacter].Setup();
    }

    private void Update()
    {
        players[currentCharacter].Execute();
    }

    private void FixedUpdate()
    {
        players[currentCharacter].FixedExecute();
    }
    #endregion

    /**********************************************/
    /***** int형, enum형 둘 다 사용 가능 *****/
    /**********************************************/

    #region Change Character & Party

    // 캐릭터 변경

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

    public void ChangeCharacter(PlayerEnums.TYPEIDS _characterType)
    {
        int index = (int)_characterType;
        if (currentCharacter == index) return;

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

    // 파티 변경

    public void ChangeParty(List<BasePlayer> _newParty)
    {
        players = _newParty;
        ChangeCharacter(0); // 첫번째 캐릭터로 변경
    }
    #endregion
}
