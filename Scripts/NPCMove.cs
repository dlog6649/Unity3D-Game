using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour {
    private Transform startPos, endPos;
    CharacterController ccontroller;

    public Node startNode { get; set; }
    public Node goalNode { get; set; }

    public ArrayList pathArray;

    public float speed = 0.1f;
    private float curSpeed;

    GameObject objStartCube, objEndCube;
    private float elapsedTime = 0.0f; // 경과 시간
    public float intervalTime = 1.0f; // 경로 탐색 사이의 시간 간격

    int i = 0;
    Node next, goal;
    Vector3 d;
    Vector3 curPosition;
    Animator anim;

    float gravity = 9.8f;

    // Use this for initialization
    void Start()
    {
        ccontroller = GetComponent<CharacterController>();
        objStartCube = GameObject.FindGameObjectWithTag("NPC"); // NPC라는 태그를 가진 오브젝트를 찾는다.
        objEndCube = GameObject.FindGameObjectWithTag("Player"); // Player라는 태그를 가진 오브젝트를 찾는다.
        pathArray = new ArrayList();
        FindPath();

        i = pathArray.Count > 1 ? 1 : 0;
        next = (Node)pathArray[i++];
        d = next.position - transform.position;
        goal = (Node)pathArray[pathArray.Count - 1];

        anim = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        curPosition = transform.position;

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();

            i = pathArray.Count != 1 ? 1 : 0;
            next = (Node)pathArray[i++];
            d = next.position - transform.position;
            goal = (Node)pathArray[pathArray.Count - 1];
        }
        if (!(Vector3.Distance(transform.position, goal.position) < 0.2f))
        {
            if (Vector3.Distance(transform.position, next.position) < 0.2f && i < pathArray.Count)
            {
                next = (Node)pathArray[i++];
                d = next.position - transform.position;
            }
            d.y = 0;
            transform.rotation = Quaternion.LookRotation(d);
            d.y -= gravity * Time.deltaTime;
            //transform.position += d * speed;
            ccontroller.Move(d * speed);
        }

        if (curPosition != transform.position)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);
    }

    void FindPath()
    {
        startPos = objStartCube.transform; // 시작큐브의 위치
        endPos = objEndCube.transform; // 종료큐브의 위치
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));
        pathArray = AStar.FindPath(startNode, goalNode);
    }

    void OnDrawGizmos()
    {
        if (pathArray == null)
            return;

        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            }
        }
    }
}