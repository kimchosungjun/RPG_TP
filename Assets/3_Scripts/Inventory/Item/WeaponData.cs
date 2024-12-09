using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ItemData
{
    // Exp & Level
    public int weaponCurrentExp;
    public int weaponMaxExp;
    public int weaponCurrentLevel;
    public int weaponMaxLevel;
    // Stat
    public float weaponPower;
    public float weaponEffectValue;
    public string weaponEffect;
    public string weaponType;
  
    // Enum 값으로 변환 후 저장할 속성
    public E_WEAPONTYPE WeaponType { get; private set; }
    public E_WEAPONEFFECT WeaponEffect { get; private set; }
    public bool IsHoldWeapon { get; private set; } = false; // Check For Sell

    public void SetWeaponType()
    {
        Enums.ConvertStringToEnum(weaponType, out E_WEAPONTYPE weaponTypeResult);
        WeaponType = weaponTypeResult;

        Enums.ConvertStringToEnum(weaponEffect, out E_WEAPONEFFECT weaponEffectResult);
        WeaponEffect = weaponEffectResult;
    }

    public override void Use()
    {
        IsHoldWeapon = true;
       
    }

    public void TakeOff()
    {
        IsHoldWeapon = false;

    }
}
