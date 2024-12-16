using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    WarriorMovementControl characterCtrl = null;
    [SerializeField] CharacterAction[] actions;
    Animator anim = null;
    void Awake()
    {
        if(characterCtrl==null) characterCtrl = GetComponentInParent<WarriorMovementControl>();
        if (characterCtrl != null) anim = characterCtrl.GetAnim;
    }

    public void AttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.AttackCooling(); }
    public void SkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.SkillCooling(); }
    public void UltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.UltimateSkillCooling(); }
    public void DashCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.DashCooling(); }  

    public void DoAction(int index)
    {
        if(actions[index].gameObject.activeSelf == false)
            actions[index].gameObject.SetActive(true); 
        actions[index].DoAction();
    }

    public void StopAction(int index)
    {
        actions[index].StopAction();
    }
}
