using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterRanking : MonoBehaviour
{
    private Canvas myCanvas, rankingCanvas;
    public GameObject obj1, obj2;

    void Awake()
    {
        myCanvas = obj1.GetComponent<Canvas>();
        rankingCanvas = obj2.GetComponent<Canvas>();
    }

    public void ChangeToRankingCanvas()
    {
        myCanvas.enabled = true;
        rankingCanvas.enabled = true;
    }
}