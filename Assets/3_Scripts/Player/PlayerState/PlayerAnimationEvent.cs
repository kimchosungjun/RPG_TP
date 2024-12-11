using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    CharacterMovement characterCtrl = null;

    void Awake()
    {
        if(characterCtrl==null) characterCtrl = GetComponentInParent<CharacterMovement>();      
    }

    public void AttackCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.AttackCooling(); }
    public void SkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.SkillCooling(); }
    public void UltimateSkillCooling() { characterCtrl.Anim.SetBool("IsAttackEnd", true); characterCtrl.UltimateSkillCooling(); }
    public void DashCooling() { characterCtrl.DashCooling(); }  
}
