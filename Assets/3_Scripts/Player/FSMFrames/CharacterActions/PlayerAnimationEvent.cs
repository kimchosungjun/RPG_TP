using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    WarriorMovement characterCtrl = null;

    void Awake()
    {
        if(characterCtrl==null) characterCtrl = GetComponentInParent<WarriorMovement>();      
    }

    public void AttackCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.AttackCooling(); }
    public void SkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.SkillCooling(); }
    public void UltimateSkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.UltimateSkillCooling(); }
    public void DashCooling() { characterCtrl.DashCooling(); }  
}
