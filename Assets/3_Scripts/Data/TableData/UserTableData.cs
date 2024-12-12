using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserTableData 
{
    [SerializeField] List<PlayerEnums.TYPEIDS> ownedCharacters = new List<PlayerEnums.TYPEIDS>(); 
    [SerializeField] List<PlayerEnums.TYPEIDS> setPartyCharacters = new List<PlayerEnums.TYPEIDS>(); 

    public UserTableData()
    {
        // 처음에는 무조건 전사만 해금
        ownedCharacters.Add(PlayerEnums.TYPEIDS.WARRIOR);
        setPartyCharacters.Add(PlayerEnums.TYPEIDS.WARRIOR);
    }
}
