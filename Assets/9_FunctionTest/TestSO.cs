using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="t",menuName ="t",order =int.MaxValue)]
public class TestSO : ScriptableObject
{
    [SerializeField]
    PlayerStat stat;

    public PlayerStat GetStat { get { return stat; } }
}
