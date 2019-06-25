using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {
    Vector3 tarPos;
    float movementSpeed = 5.0f;
    float rotSpeed = 2.0f;
    float minX, maxX, minZ, maxZ;


	// Use this for initialization
	void Start () {
        minX = -45.0f;
        maxX = 45.0f;
        minZ = -45.0f;
        maxZ = 45.0f;

        // 돌아다닐 지점 가져오기
        GetNextPosition();
	}
	
	// Update is called once per frame
	void Update () {
        // 목표 지점 근처에 있는지 검사
        if (Vector3.Distance(tarPos, transform.position) <= 5.0f)
            GetNextPosition();

        // 목적지를 향하는 회전 값 설정
        Quaternion tarRot = Quaternion.LookRotation(tarPos - transform.position);

        // 회전과 변환 업데이트
        transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, rotSpeed * Time.deltaTime);
        transform.Translate(new Vector3(0, 0, movementSpeed * Time.deltaTime));
	}

    void GetNextPosition()
    {
        tarPos = new Vector3(Random.Range(minX, maxX), 0.5f, Random.Range(minZ, maxZ));
    }
}
