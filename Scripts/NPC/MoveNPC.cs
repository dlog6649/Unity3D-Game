using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNPC : MonoBehaviour {
    public GameObject thePath;
    public float moveSpeed = 1.0f;
    public float intervalTime = 1.0f; // 경로 탐색 사이의 시간 간격

    private WaypointMaster wpMaster;
    private CharacterController ccontroller;
    private Vector3 startPos, endPos, nextPos, moveDirection;
    private Vector3 destination;
    private float elapsedTime = 0.0f; // 경과 시간
    private int resultPath = -1;
    private int idx = 0;
    private float gravity = 9.8f;

    public void SetResultPath(int setNum)
    {
        resultPath = setNum;
    }
    
    void Awake()
    {
        wpMaster = thePath.GetComponent<WaypointMaster>();
        ccontroller = GetComponent<CharacterController>();
    }

    public int GoToLocation()
    {
        moveSpeed = 3f;
        // 이미 길을 찾은 상태라면 더이상 찾지 않는다.
        if (resultPath != 1)
        {
            destination = GameObject.FindGameObjectWithTag("Player").transform.position;

            //elapsedTime += Time.deltaTime;
            //if (elapsedTime >= intervalTime)
            //{
            //    elapsedTime = 0.0f;
            resultPath = wpMaster.FindPath(transform.position, destination);

            switch (resultPath)
            {
                case 0:
                    print("can't find path");
                    return 0;
                case 1:
                    idx = wpMaster.pathList.Count > 1 ? 1 : 0;
                    nextPos = wpMaster.pathList[idx++];
                    moveDirection = nextPos - transform.position;
                    break;
                default: break;
            }
        } // 길을 찾은상태이고(1), 목표와 일정거리보다 멀다면 계속 이동한다.
        if (resultPath == 1)// && !(Vector3.Distance(transform.position, destination) < 1.0f))
        {
            if (Vector3.Distance(transform.position, nextPos) < 0.5f && idx < wpMaster.pathList.Count)
            {
                nextPos = wpMaster.pathList[idx++];
                moveDirection = nextPos - transform.position;
            }
            moveDirection.y = 0;
            Vector3 targetPos = transform.position + moveDirection;
            Vector3 framePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            Vector3 frameDir = framePos - transform.position;
            transform.rotation = Quaternion.LookRotation(frameDir);
            //frameDir.y -= gravity * Time.deltaTime;
            ccontroller.Move(frameDir + Physics.gravity);
        }
        return 1;
    }

    public void FollowTarget(Vector3 endPos)
    {
        moveSpeed = 5.0f;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            resultPath = wpMaster.FindPath(transform.position, endPos);
            switch (resultPath)
            {
                case 0:
                    print("can't find path");
                    return;
                case 1:
                    idx = wpMaster.pathList.Count > 1 ? 1 : 0;
                    nextPos = wpMaster.pathList[idx++];
                    moveDirection = nextPos - transform.position;
                    break;
                default: break;
            }
        }
        if (resultPath == 1)
        {
            if (Vector3.Distance(transform.position, nextPos) < 0.5f && idx < wpMaster.pathList.Count)
            {
                nextPos = wpMaster.pathList[idx++];
                moveDirection = nextPos - transform.position;
            }
            moveDirection.y = 0;
            Vector3 targetPos = transform.position + moveDirection;
            Vector3 framePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            Vector3 frameDir = framePos - transform.position;
            transform.rotation = Quaternion.LookRotation(frameDir);
            frameDir.y -= gravity * Time.deltaTime;
            ccontroller.Move(frameDir + Physics.gravity);
        }
    }

    public void Patrol(Vector3 nextPatrolPosition)
    {
        moveSpeed = 1.0f;
        if (resultPath != 1)
        {
            resultPath = wpMaster.FindPath(transform.position, nextPatrolPosition);

            switch (resultPath)
            {
                case 0:
                    print("can't find path");
                    return;
                case 1:
                    idx = wpMaster.pathList.Count > 1 ? 1 : 0;
                    nextPos = wpMaster.pathList[idx++];
                    moveDirection = nextPos - transform.position;
                    break;
                default: break;
            }


        } // 길을 찾은 상태라면 이동을 시작한다.
        if (resultPath == 1)
        {
            if (Vector3.Distance(transform.position, nextPos) < 0.5f && idx < wpMaster.pathList.Count)
            {
                nextPos = wpMaster.pathList[idx++];
                moveDirection = nextPos - transform.position;
            }
            moveDirection.y = 0;
            Vector3 targetPos = transform.position + moveDirection;
            Vector3 framePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            Vector3 frameDir = framePos - transform.position;
            transform.rotation = Quaternion.LookRotation(frameDir);
            //frameDir.y -= gravity * Time.deltaTime;
            ccontroller.Move(frameDir + Physics.gravity);
        }
    }
}
