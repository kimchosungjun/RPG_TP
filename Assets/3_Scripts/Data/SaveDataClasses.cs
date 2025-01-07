using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveDataGroup
{
    #region Origin
    public class SaveDataClasses { }
    #endregion

    public class QuestSaveData
    {

    }

    public class NPCSaveData
    {
        public int dialogueIndex;
        public int currentQuestIndex = -1;
        public List<int> clearQuestIDSet;
    }
}


