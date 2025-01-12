using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveDataGroup
{
    #region Origin
    public class SaveDataClasses { }
    #endregion

    public class PlayerSaveDataGroup
    {
        public List<int> cuurrentPlayerParty;
        //public List<int> cuurrentPlayerParty2;
    }

    public class QuestSaveDataGroup
    {

    }

    public class NPCSaveDataGroup
    {
        public int dialogueIndex;
        public int currentQuestIndex = -1;
        public List<int> clearDialogues;
    }
}


