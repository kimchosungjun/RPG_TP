using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NPCSOData", menuName ="SOData/NPC")]
public class NPCSOData : ScriptableObject
{
    [SerializeField] int[] questIDSet;
}
