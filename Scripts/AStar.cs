using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    public static PriorityQueue closedList, openList;

    private static float HeuristicEstimateCost(Node curNode, Node goalNode) // 발견적 견적 비용
    {
        Vector3 vecCost = curNode.position - goalNode.position; // 현재 노드 위치 - 목적 노드 위치
        return vecCost.magnitude; // vecCost의 규모 반환
    }

    public static ArrayList FindPath(Node start, Node goal)
    {
        openList = new PriorityQueue(); // 열린 리스트 생성
        openList.Push(start);
        start.nodeTotalCost = 0.0f; // G 시작위치에서 현재 노드까지의 이동비용
        start.estimatedCost = HeuristicEstimateCost(start, goal); // F 최종 점수

        closedList = new PriorityQueue(); // 닫힌 리스트 생성
        Node node = null;

        while (openList.Length != 0) // 열린리스트에 노드가 남아있는 동안 반복
        {
            node = openList.First(); // 오름차순 정렬된 열린리스트의 첫번째 노드를 가져온다. (가장 값이 낮은 노드)

            if (node.position == goal.position) // 현재노드가 목적지노드인지 확인한다.
            {
                return CalculatePath(node);
            }
            
            ArrayList neighbours = new ArrayList(); // 이웃노드를 저장하기 위해 ArrayList를 생성한다.

            GridManager.instance.GetNeighbours(node, neighbours); // 현재노드의 이웃노드들을 이웃 리스트에 추가한다

            for (int i = 0; i < neighbours.Count; i++)
            {
                Node neighbourNode = (Node)neighbours[i];
                if (!closedList.Contains(neighbourNode)) // 닫힌 리스트에 들어있지 않으면
                {
                    float cost = HeuristicEstimateCost(node, neighbourNode); // 현재노드에서 이웃 노드까지의 비용

                    float totalCost = node.nodeTotalCost + cost; // 시작노드~현재노드의 거리비용 + 노드~이웃노드의 거리비용 G
                    float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal); // 이웃노드~목표노드의 거리비용 H

                    neighbourNode.nodeTotalCost = totalCost; // 이웃노드의 G를 초기화한다.
                    neighbourNode.parent = node; // 이웃노드의 부모노드를 현재노드로 한다.
                    neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost; // F = G + H

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Push(neighbourNode);
                    }
                }
            }
            closedList.Push(node); // 현재 노드를 closedList에 추가한다.
            openList.Remove(node); // 그리고 openList에서는 제거한다.
        }

        if (node.position != goal.position) // 열린리스트가 비었는데 현재노드가 목표노드가 아닌 경우 에러메시지 출력
        {
            Debug.LogError("Goal Not Found");
            return null;
        }
        return CalculatePath(node); // 현재노드가 목표노드에 도착한 경우 경로를 계산한다.
    }

    private static ArrayList CalculatePath(Node node) // 부모노드를 역추적해서 경로를 계산한다.
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse(); // 리스트를 반전시킨다.
        return list;
    }
}