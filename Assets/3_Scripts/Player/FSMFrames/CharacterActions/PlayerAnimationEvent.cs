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

    public void AnimAttackCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.AttackCooling(); }
    public void AnimSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.SkillCooling(); }
    public void AnimUltimateSkillCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.UltimateSkillCooling(); }
    public void AnimDashCooling() { anim.SetInteger("States", (int)PlayerEnums.STATES.MOVEMENT); characterCtrl.DashCooling(); }  

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
