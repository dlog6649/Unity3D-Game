using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sense : MonoBehaviour {
    private float detectionRate = 1.0f;
    private int FieldOfView = 60;
    private int ViewDistance = 21;

    private Transform playerTrans;
    private Vector3 rayDirection;
    private float elapsedTime = 0.0f;
    private AI_Agent ai;
    
    // Use this for initialization
    void Start () {
        elapsedTime = 0.0f;
        ai = GetComponent<AI_Agent>();

        // 플레이어 위치 찾기
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;

        // 감시 주기 내에 있다면 시각 감시를 수행한다.
        if (elapsedTime >= detectionRate)
        {
            DetectAspect();
            elapsedTime = 0.0f;
        }
    }

    // NPC의 시각 감시
    void DetectAspect()
    {
        RaycastHit hit;

        // 현재 위치에서 플레이어 위치로의 방향
        rayDirection = playerTrans.position - transform.position;

        // 인공지능 캐릭터의 전방 벡터 그리고 플레이어와 인공지능 간의 방향 벡터 사이의 각도를 검사한다.
        if ((Vector3.Angle(rayDirection, transform.forward)) < FieldOfView)
        {
            // 플레이어가 필드 내 보이는 영역에 있는지 감지한다.
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    print("Player Detected");
                    ai.ChangeBehavior(Behaviors.Chase);
                }
            }
        }
    }

    // 촉각
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Player Touch Detected");
            ai.isInRange = true;
            ai.ChangeBehavior(Behaviors.Chase);
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
