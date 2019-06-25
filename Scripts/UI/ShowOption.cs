using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOption : MonoBehaviour {

    Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}
