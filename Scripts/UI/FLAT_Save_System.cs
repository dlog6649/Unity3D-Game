using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class FLAT_Save_System : MonoBehaviour {
    public string sFileName;
    public string sDirectory;

    public GameObject Player;
    
    void WirteToFile(string file = "")
    {
        if (file != "")
            sFileName = file;

        if(File.Exists(sDirectory + sFileName))
            DeleteFile(sFileName);

        using (StreamWriter sw = new StreamWriter(sDirectory + sFileName))
        {
            sw.WriteLine(PlayerPrefs.GetInt("PlayerDeaths").ToString());
            sw.WriteLine(PlayerPrefs.GetFloat("PlayerTimePlayed").ToString());
            sw.WriteLine(Player.transform.position.x.ToString());
            sw.WriteLine(Player.transform.position.y.ToString());
            sw.WriteLine(Player.transform.position.z.ToString());
        }
    }

    void DeleteFile(string file = "")
    {
        File.Delete(sDirectory + file);
    }

    void ReadFile(string file = "")
    {
        if (file != "")
            sFileName = file;

        using (StreamReader sr = new StreamReader(sDirectory + sFileName))
        {
            int deaths = Convert.ToInt32(sr.ReadLine());
            float ptime = Convert.ToSingle(sr.ReadLine());
            float x = Convert.ToSingle(sr.ReadLine());
            float y = Convert.ToSingle(sr.ReadLine());
            float z = Convert.ToSingle(sr.ReadLine());
            Player.transform.position = new Vector3(x, y, z);
        }
    }
}
