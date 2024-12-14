using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    WarriorMoveCtrl characterCtrl = null;
    [SerializeField] CharacterAction[] actions;
    void Awake()
    {
        if(characterCtrl==null) characterCtrl = GetComponentInParent<WarriorMoveCtrl>();      
    }

    public void AttackCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.AttackCooling(); }
    public void SkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.SkillCooling(); }
    public void UltimateSkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.UltimateSkillCooling(); }
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
