using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : IComparable { // F(총 비용) = G + H
    public float nodeTotalCost; // G 시작 위치에서 현재 노드까지의 이동 비용
    public float estimatedCost; // F = H 현재 노드에서 대상 목표 노드까지의 총 추정 비용 (견적 비용) + G
    public bool bObstacle; // b장애물
    public Node parent;
    public Vector3 position;
    public float height;

    public Node() // 생성자
    {
        this.estimatedCost = 0.0f;
        this.nodeTotalCost = 1.0f;
        this.bObstacle = false;
        this.parent = null;
    }

    public Node(Vector3 pos) // 생성자
    {
        this.estimatedCost = 0.0f;
        this.nodeTotalCost = 1.0f;
        this.bObstacle = false;
        this.parent = null;
        this.position = pos;
    }

    public void MarkAsObstacle()
    {
        this.bObstacle = true;
    }

    public int CompareTo(object obj)
    {
        Node node = (Node)obj;
        // 음수 값은 오브젝트가 정렬된 상태에서 현재보다 앞에 있음을 의미한다.
        if (this.estimatedCost < node.estimatedCost)
            return -1;
        // 양수 값은 뒤에 있음을 의미한다.
        else if (this.estimatedCost > node.estimatedCost)
            return 1;
        else
            return 0;
    }
}
