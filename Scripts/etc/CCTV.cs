using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour {
    
    private int FieldOfView = 60;
    private int ViewDistance = 15;

    private Transform playerTrans;
    private Vector3 rayDirection;

    private float elapsedTime = 0.0f;
    private float detectionRate = 1.0f;

    private AI_Agent ai;
    private GameObject obj;
    private MoveNPC moveNpc;

    private int timeToDeath = 0;

    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("NPC");
        ai = obj.GetComponent<AI_Agent>();
        moveNpc = obj.GetComponent<MoveNPC>();

        // 플레이어 위치 찾기
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime;

        // 감시 주기 내에 있다면 시각 감시를 수행한다.
        if (elapsedTime >= detectionRate)
        {
            elapsedTime = 0.0f;
            DetectAspect();
        }
    }

    // CCTV의 시각 감시
    void DetectAspect()
    {
        RaycastHit hit;

        // 현재 위치에서 플레이어 위치로의 방향
        rayDirection = playerTrans.position - transform.position;

        // CCTV의 전방 벡터 그리고 플레이어와 CCTV의 방향 벡터 사이의 각도를 검사한다.
        if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfView)
        {
            // 플레이어가 필드 내 보이는 영역에 있는지 감지한다.
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                // 만약 플레이어가 범위내에 들어왔다면 timeToDeath 변수의 값을 1초마다 1씩 증가시킨다.
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("RightDoor") || hit.collider.CompareTag("LeftDoor"))
                {
                    timeToDeath++;
                    print("timeToDeath: " + timeToDeath);
                    // 3초간 cctv의 범위내에 머물러있고, npc가 지금 추격상태가 아니라면 정찰을 오게 만든다.
                    if (timeToDeath >= 3 && ai.aiBehaviors != Behaviors.Chase)
                    {
                        timeToDeath = 0;
                        moveNpc.SetResultPath(-1);
                        ai.isSuspicious = true;
                        ai.SetPlayerLocation(transform.position);
                        ai.ChangeBehavior(Behaviors.Guard);
                    }
                }
                else
                {
                    timeToDeath = 0;
                }
            }
            else
            {
                timeToDeath = 0;
            }
        }
        else
        {
            timeToDeath = 0;
        }
    }

    void OnDrawGizmos()
    {
        if (playerTrans == null) return;

        Debug.DrawLine(transform.position, playerTrans.position, Color.red);

        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        // 시야 시각화
        Vector3 leftRayPoint = frontRayPoint;
        leftRayPoint.x += FieldOfView * 0.5f;

        Vector3 rightRayPoint = frontRayPoint;
        rightRayPoint.x -= FieldOfView * 0.5f;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }
}
