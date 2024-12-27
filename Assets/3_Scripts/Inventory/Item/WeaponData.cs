using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemEnums;

public class WeaponData : ItemData
{
    // Exp & Level
    public int weaponCurrentExp;
    public int weaponMaxExp;
    public int weaponCurrentLevel;
    public int weaponMaxLevel;
    // Stat
    public float attackValue;
    public float effectValue;
    public int weaponEffect;
    public int weaponType; // staff, bow..
  
    // Enum 값으로 변환 후 저장할 속성
    public WEAPONTYPE WeaponType { get; private set; }
    public WEAPONEFFECT WeaponEffect { get; private set; }
    public bool IsHoldWeapon { get; private set; } = false; // Check For Sell

    public override void Use()
    {
        IsHoldWeapon = true;
       
    }

    public void TakeOff()
    {
        IsHoldWeapon = false;

    }
}
