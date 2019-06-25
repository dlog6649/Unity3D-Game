using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour {
    private Canvas canvas;
    private GameObject obj;
    private GoalFlagCheck gfc;


    void Start()
    {
        canvas = GetComponent<Canvas>();
        obj = GameObject.FindGameObjectWithTag("Goal");
        gfc = obj.GetComponent<GoalFlagCheck>();
    }

    void Update()
    {
        if (gfc.goalFlag)
        {
            canvas.enabled = true;
        }
    }
}
