using UnityEngine;

public class GroupActions : CharacterAction
{
    [SerializeField] CharacterAction[] actions;
    public override void DoAction()
    {
        int cnt = actions.Length;
        for(int i=0; i<cnt; i++)
        {
            actions[i].DoAction();  
        }
    }

    public override void StopAction()
    {
        int cnt = actions.Length;
        for (int i = 0; i < cnt; i++)
        {
            actions[i].StopAction();
        }
    }
}
