using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorForNPC : MonoBehaviour {
    private float timeLeft = 0;
    private Transform currentDoor;
    private bool isDoor;
    private bool leftDoor;
    private bool rightDoor;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftDoor"))
        {
            isDoor = true;
            leftDoor = true;
            currentDoor = other.transform.parent;
        }
        if (other.CompareTag("RightDoor"))
        {
            isDoor = true;
            rightDoor = true;
            currentDoor = other.transform.parent;
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        if (isDoor)
        {
            OpenAndCloseDoor();
        }
    }

    public void OpenAndCloseDoor()
    {
        timeLeft += Time.deltaTime;
        
        if (leftDoor)
        {
            currentDoor.localRotation = Quaternion.Slerp(currentDoor.localRotation, Quaternion.Euler(0, 150, 0), timeLeft);
        }
        else if (rightDoor)
        {
            currentDoor.localRotation = Quaternion.Slerp(currentDoor.localRotation, Quaternion.Euler(0, -150, 0), timeLeft);
        }
        if (timeLeft > 0.3)
        {
            timeLeft = 0;
            isDoor = false;
            leftDoor = false;
            rightDoor = false;
        }
    }
}
