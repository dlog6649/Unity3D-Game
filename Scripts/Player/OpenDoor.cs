using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {
    private float timeLeft = 0;
    private RaycastHit hit;
    private Transform currentDoor;
    private bool isOpen;
    private bool isDoor;
    private bool leftDoor;
    private bool rightDoor;
    public Transform cam;
    private LayerMask leftDoorMask = 1 << 8;
    private LayerMask rightDoorMask = 1 << 10;

    private GameObject npc;
    private AI_Agent ai;
    private MoveNPC moveNpc;
    private float verticalNpc;

    void Start()
    {
        npc = GameObject.FindGameObjectWithTag("NPC");
        ai = npc.GetComponent<AI_Agent>();
        moveNpc = npc.GetComponent<MoveNPC>();
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.F) && timeLeft == 0.0f)
        {
            CheckDoor();
        }
        if (isDoor)
        {
            OpenAndCloseDoor();
        }
	}

    public void CheckDoor()
    {
        if(Physics.Raycast(cam.position, cam.forward, out hit, 1.5f, leftDoorMask))
        {
            isOpen = false;
            if(hit.transform.parent.localRotation.eulerAngles.y > 45)
            {
                isOpen = true;
            }
            isDoor = true;
            leftDoor = true;
            currentDoor = hit.transform.parent;
        }

        if (Physics.Raycast(cam.position, cam.forward, out hit, 1.5f, rightDoorMask))
        {
            isOpen = false;
            if (hit.transform.parent.localRotation.eulerAngles.y != 0)
            {
                isOpen = true;
            }
            isDoor = true;
            rightDoor = true;
            currentDoor = hit.transform.parent;
        }
    }

    public void OpenAndCloseDoor()
    {
        timeLeft += Time.deltaTime;

        if (isOpen)
        {
            currentDoor.localRotation = Quaternion.Slerp(currentDoor.localRotation, Quaternion.Euler(0, 0, 0), timeLeft);
        }
        else if(!isOpen && leftDoor)
        {
            currentDoor.localRotation = Quaternion.Slerp(currentDoor.localRotation, Quaternion.Euler(0, 120, 0), timeLeft);
        }
        else if (!isOpen && rightDoor)
        {
            currentDoor.localRotation = Quaternion.Slerp(currentDoor.localRotation, Quaternion.Euler(0, -120, 0), timeLeft);
        }
        if (timeLeft > 1)
        {
            timeLeft = 0;
            isDoor = false;
            leftDoor = false;
            rightDoor = false;

            // 플레이어와 npc의 세로축의 거리를 비교해서 2보다 낮으면 같은층에 있는것이다.
            if (transform.position.y >= ai.transform.position.y)
            {
                verticalNpc = transform.position.y - ai.transform.position.y;
            }
            else
            {
                verticalNpc = ai.transform.position.y - transform.position.y;
            }

            // 추격 상태가 아니고, 거리가 15이하고, 같은층에 있는 npc의 일정거리 내에서 문을 여닫을시, npc가 정찰오게 만듦.
            if (Vector3.Distance(transform.position, npc.transform.position) <= 15.0f && ai.aiBehaviors != Behaviors.Chase && verticalNpc <= 2.2f)
            {
                moveNpc.SetResultPath(-1);
                ai.isSuspicious = true;
                ai.SetPlayerLocation(transform.position);
                ai.ChangeBehavior(Behaviors.Guard);
            }
        }
    }
}
