using System.Collections.Generic;

public class UniqueIDMaker 
{
    private static int uniqueID = 0;
    public static HashSet<int> uniqueIDSet = new HashSet<int>();

    public static int GetUniqueID()
    {
        while (uniqueIDSet.Contains(uniqueID))
        {
            uniqueID += 1;
        }
        uniqueIDSet.Add(uniqueID);
        return uniqueID;
    }

    public static void RemoveID(int _id)
    {
        if(uniqueIDSet.Contains(_id))
            uniqueIDSet.Remove(_id);
    }

    public static void AddID(int _id)
    {
        if (uniqueIDSet.Contains(_id))
            return;
        uniqueIDSet.Add(_id);
    }
}
