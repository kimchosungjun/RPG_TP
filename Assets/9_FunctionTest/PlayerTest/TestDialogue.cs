using UnityEngine;
using PlayerEnums;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] WarriorMovement characterCtrl;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, 100, 100), "대화하기"))
        {
            characterCtrl?.TestDialogue(this.transform);
        }

        if (GUI.Button(new Rect(100, 0, 100, 100), "대화풀기"))
        {
            characterCtrl?.ChangeState(PlayerEnums.STATES.MOVEMENT);
        }
    }
}
