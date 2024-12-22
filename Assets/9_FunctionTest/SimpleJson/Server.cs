using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    [SerializeField] ParseClassList ParseClassList2;
    string Http = "http://58.78.211.182:3000/"; //ip와 port(통로)로 이루어짐 => domain과 연결

    string ConnectUrl = "process/dbconnect";
    string DisConnectUrl = "process/dbdisconnect";
    string UserSelectUrl = "process/userselect";

    IEnumerator DBPost(string url, string num) 
    {
        ParseClassList2 = new ParseClassList();
        ParseClass parse = new ParseClass();
        WWWForm form = new WWWForm();
        form.AddField("num", num);
        UnityWebRequest www = UnityWebRequest.Post(url,form);
        yield return www.SendWebRequest();
        //ParseClassList = JsonUtility.FromJson<ParseClassList>(www.downloadHandler.text);
        JSONNode node = JSONNode.Parse(www.downloadHandler.text);
        
        for(int i=0; i<node.Count; i++)
        {
            ParseClassList2.parseList.Add(i,JsonUtility.FromJson<ParseClass>(node[i].ToString()));
            ParseClassList2.list.Add(JsonUtility.FromJson<ParseClass>(node[i].ToString()));
        }

        //JsonUtility.ToJson(node);
        //Debug.Log(www.downloadHandler.text);
        //get, post, pull 방식이 존재한다.
        //get
    }

    public void OnBtnConnect()
    {
        StartCoroutine(DBPost(Http+ConnectUrl,"dev"));
    }

}

[Serializable]
public class ParseClassList
{
    [SerializeField] public Dictionary<int, ParseClass> parseList = new Dictionary<int, ParseClass>();
    [SerializeField] public List<ParseClass> list = new List<ParseClass>(); 
}

[Serializable]
public class ParseClass
{
    [SerializeField] int id;
    [SerializeField] string account;
    [SerializeField] int platformType;
    [SerializeField] string platformToken;
    [SerializeField] string userName;
    [SerializeField] int deviceID;
    [SerializeField] string createdTime;
    [SerializeField] string lastConnectedTime;
}