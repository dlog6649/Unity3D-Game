using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFlagCheck : MonoBehaviour {
    public bool goalFlag = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            goalFlag = true;
        }
    }
}
