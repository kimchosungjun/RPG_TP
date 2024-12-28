using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeData : ItemData
{
    public int effect;
    public float effectValue;
    public float maintainTime;

    int maxCnt = 999;
    public int GetMaxCnt { get { return maxCnt; } }

    public override void Remove()
    {
        base.Remove();
    }

    public override void Use()
    {
        base.Use(); 
    }
}
