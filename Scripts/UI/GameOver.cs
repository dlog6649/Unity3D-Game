using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour {
    Canvas canvas;
    private GameObject obj;
    private PlayerController pc;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        obj = GameObject.FindGameObjectWithTag("Player");
        pc = obj.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (pc.deathFlag)
        {
            canvas.enabled = true;
        }
    }
}
