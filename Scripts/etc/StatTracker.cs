using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTracker : MonoBehaviour {
    //private float pTimePlayed = 0.0f; // 변수앞의 p는 player라는 의미
    //private int pDeaths = 0;

    public int playTime = 0;
    private int seconds = 0;
    private int minutes = 0;
    private int hours = 0;

    void Start () {
        StartCoroutine("PlayTimer");
	}

    private IEnumerator PlayTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            playTime += 1;
            seconds = (playTime % 60);
            minutes = (playTime / 60) % 60;
            hours = (playTime / 3600) % 24;
        }
    }

    public string PlayerTimePlayed()
    {
        string temp = hours + "시간 " + minutes + "분 " + seconds + "초";
        return temp;
    }



    /*
    void SetStat(string stat, int intValue = 0, float fltValue = 0f)
    {
        switch (stat)
        {
            //case "Deaths":
            //    pDeaths += intValue;
            //    break;
            case "TimePlayed":
                pTimePlayed += fltValue;
                break;
            default:
                break;
        }
    }

    void ResetStats()
    {
        //pDeaths = 0;
        pTimePlayed = 0.0f;
    }

    void ResetAllPrefs()
    {
        //PlayerPrefs.SetInt("PlayerDeaths", 0);
        PlayerPrefs.SetFloat("PlayerTimePlayed", 0.0f);
        PlayerPrefs.Save();
    }

    void SaveAllPrefs()
    {
        //PlayerPrefs.SetInt("PlayerDeaths", pDeaths);
        PlayerPrefs.SetFloat("PlayerTimePlayed", pTimePlayed);
        PlayerPrefs.Save();
    }*/
}
