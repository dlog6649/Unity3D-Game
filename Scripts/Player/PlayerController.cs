using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour { // 시야 위아래로 180도 이상 안넘어가게 코드 추가.
    float turnSpeed = 80.0f;
    float moveSpeed = 5.0f; // units per second
    float jumpSpeed = 5.0f;
    float gravity = 9.8f;
    float vSpeed = 0.0f; // current vertical velocity
    CharacterController controller;
    Vector3 vel;
    private GameObject obj;
    private AI_Agent aa;
    private float rotSpeed = 1.0f;
    private float timeOut = 4.0f;
    private float elapsedTime = 0f;
    public bool deathFlag = false;
    public bool isCrouch = false;
    private MoveNPC moveNpc;

    private Vector3 curPos;
    private float verticalNpc;
        

    void Start () {
        controller = GetComponent<CharacterController>();
        obj = GameObject.FindGameObjectWithTag("NPC");
        aa = obj.GetComponent<AI_Agent>();
        moveNpc = obj.GetComponent<MoveNPC>();
    }
	// Update is called once per frame
	void Update () {
        if (!aa.isInRange)
        {
            // 내가 최근에 이동했는지 확인하기 위해서 사용
            curPos = transform.position;
            
            if (Input.GetKey("left ctrl"))
            {
                moveSpeed = 1.0f;
                isCrouch = true;
            }
            else
            {
                moveSpeed = 5.0f;
                isCrouch = false;
            }

            transform.Rotate(0, Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0);
            vel = transform.right * Input.GetAxisRaw("Horizontal");
            vel += transform.forward * Input.GetAxisRaw("Vertical");
            vel = vel.normalized * moveSpeed;
            //vel *= moveSpeed;

            

            if (controller.isGrounded)
            {
                vSpeed = 0; // grounded character has vSpeed = 0...
                if (Input.GetKeyDown("space"))
                { // unless it jumps:
                    vSpeed = jumpSpeed;
                }
            }
            // apply gravity acceleration to vertical speed:
            vSpeed -= gravity * Time.deltaTime;
            vel.y = vSpeed; // include vertical speed in vel
                            // convert vel to displacement and Move the character:
            controller.Move(vel * Time.deltaTime);

            // 플레이어와 npc의 세로축의 거리를 비교해서 2보다 낮으면 같은층에 있는것이다.
            if (transform.position.y >= aa.transform.position.y)
            {
                verticalNpc = transform.position.y - aa.transform.position.y;
            }
            else
            {
                verticalNpc = aa.transform.position.y - transform.position.y;
            }
            // 만약 내가 움직였고, 앉은상태가 아니었고, npc와의 거리가 10이하고, 추격상태가 아니고, 호텔에서 너무 멀리 떨어진게 아니고 다른층이 아니라면 npc가 소리를 듣고 정찰오게 만든다.
            if(curPos != transform.position && isCrouch == false && Vector3.Distance(aa.transform.position, transform.position) <= 10.0f && aa.aiBehaviors != Behaviors.Chase 
                && transform.position.x < 30f && transform.position.x > -30f && transform.position.z < 40f && transform.position.z > -40f && verticalNpc <= 2.2f)
            {
                moveNpc.SetResultPath(-1);
                aa.isSuspicious = true;
                aa.SetPlayerLocation(transform.position);
                aa.ChangeBehavior(Behaviors.Guard);
            }
        }
        else // 게임 종료 전에 애니메이션 추가.
        {
            elapsedTime += Time.deltaTime;

            // 목적지를 향하는 회전 값 설정
            Vector3 dir = obj.transform.position - transform.position;
            dir.y = 0;
            Quaternion tarRot = Quaternion.LookRotation(dir);
            // 회전과 변환 업데이트
            transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, rotSpeed * Time.deltaTime);
            
            if (elapsedTime >= timeOut)
            {
                deathFlag = true;
            }
        }
    }
}
