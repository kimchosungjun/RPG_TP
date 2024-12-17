using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterDropItem
{
    protected int dropID;
    protected int[] dropItemIDs;
    protected float[] dropProbabilities;
    protected int dropGold;
    protected int dropExp;
    protected int minQuantity;
    protected int maxQuantity;
    protected float[] quantityProbabilities;
    
    public void SetArraySize(int _dropItemCnt, int _quantityCnt)
    {
        dropItemIDs = new int[_dropItemCnt];
        dropProbabilities = new float[_dropItemCnt];    
        quantityProbabilities = new float[_quantityCnt];
    }

    public void SetDropData(int _dropID, int[] _dropItemIDs, float[] _dropProbabilities, int _dropGold, int _dropExp, int _minQuantity, int _maxQuantitiy)
    {

    }
}
