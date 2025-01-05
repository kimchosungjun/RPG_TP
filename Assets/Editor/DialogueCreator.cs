using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(DialougeDataHolder))]
public class DialogueCreator : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DialougeDataHolder holder = (DialougeDataHolder)target; 
        
        if(GUILayout.Button("Generate DialogueData"))
        {
            holder.CreateData();    
        }   
    }
}
