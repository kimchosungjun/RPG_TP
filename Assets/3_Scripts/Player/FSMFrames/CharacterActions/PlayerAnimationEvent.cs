using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    WarriorMovementControl characterCtrl = null;
    [SerializeField] CharacterAction[] actions;
    void Awake()
    {
        if(characterCtrl==null) characterCtrl = GetComponentInParent<WarriorMovementControl>();      
    }

    public void AttackCooling() { characterCtrl.GetAnim.SetBool("IsAttackEnd", true); characterCtrl.AttackCooling(); }
    public void SkillCooling() { characterCtrl.GetAnim.SetBool("IsAttackEnd", true); characterCtrl.SkillCooling(); }
    public void UltimateSkillCooling() { characterCtrl.GetAnim.SetBool("IsAttackEnd", true); characterCtrl.UltimateSkillCooling(); }
    public void DashCooling() { characterCtrl.DashCooling(); }  

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
