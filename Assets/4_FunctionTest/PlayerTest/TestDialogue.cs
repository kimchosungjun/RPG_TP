using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] CharacterCtrl characterCtrl;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "대화하기"))
        {
            characterCtrl?.TestDialogue(this.transform);
        }

        if (GUI.Button(new Rect(100, 0, 100, 100), "대화풀기"))
        {
            characterCtrl?.ChangeState(E_PLAYER_STATES.MOVEMENT);
        }
    }
}
