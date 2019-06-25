using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Net;

public class SendRanking : MonoBehaviour {
    private string userName;
    private string playTime;

    public GameObject inputField;

    private InputField nickName;
    private StatTracker stats;
    
	void Awake () {
        nickName = inputField.GetComponent<InputField>();
        stats = GetComponent<StatTracker>();
	}
	
    public void SendToRankingServer()
    {
        userName = nickName.text;
        playTime = stats.PlayerTimePlayed();
        
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:55000/ranking");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = "{\"username\":\"" + userName + "\", \"time\":\"" + playTime + "\"}";
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
        }
    }
}