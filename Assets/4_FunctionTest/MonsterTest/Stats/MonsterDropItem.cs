using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterDropItem
{
    protected int monsterID;
    protected List<int> dropItemIDList;
    protected List<int> dropRateList;
    protected int[] goldRange = new int[2];

    public MonsterDropItem(int _monsterID, List<int> _dropItemIDList, List<int> _dropRateList, int _goldRangeStart, int _goldRangeEnd)
    {
        monsterID = _monsterID;
        dropItemIDList = _dropItemIDList;
        dropRateList = _dropRateList;
        goldRange[0] = _goldRangeStart;
        goldRange[1] = _goldRangeEnd + 1;
    }

    public void GetDrop()
    {
        //Queue<Item>();
        // 오른쪽 하단에 획득한 물품창을 만들 생각 => Queue로 전달 (골드, 아이템)


        int getGold = GetGold();
        // 골드에 해당 골드만큼 추가
    }

    public int GetGold()
    {
        int getGold = Random.Range(goldRange[0], goldRange[1]);
        return getGold;
    }

    /// <summary>
    /// 반환값은 나중에 수정
    /// </summary>
    /// <returns></returns>
    public List<int> GetItem()
    {
        // List<Item> getItemList ;
        int percent = 0;
        int listCnt = dropItemIDList.Count;
        for (int i = 0; i < listCnt; i++)
        {
            percent = Random.Range(0, 101);
            if (percent <= dropRateList[i])
            {
                // 아이템 리스트에 추가
            }
        }


        // 나중에 수정
        return dropItemIDList;
    }
}
