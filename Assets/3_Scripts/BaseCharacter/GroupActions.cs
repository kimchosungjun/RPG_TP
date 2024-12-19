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
        actions[0].StartCoolDown();
    }

    public override void StopAction()
    {
        int cnt = actions.Length;
        for (int i = 0; i < cnt; i++)
        {
            actions[i].StopAction();
        }
    }

    public override bool IsCoolDown()
    {
       return actions[0].IsCoolDown();   
    }

    public override float GetActionCoolTime() { return actions[0].GetActionCoolTime();}
}
