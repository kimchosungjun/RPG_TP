using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataLoader : MonoBehaviour
{
    [SerializeField]
    AccountSaveData data;

    private void Awake()
    {
        data  =new AccountSaveData();
        data.GetPlayerParty();
    }
}
