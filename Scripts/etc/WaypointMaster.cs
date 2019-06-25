using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMaster : MonoBehaviour {
    private GameObject[] waypoints;
    private List<Waypoint> waypointList; // 모든 웨이포인트들의 리스트
    private List<Waypoint> openList;
    private List<Waypoint> closedList;
    private int waypointID = 1;
    private int layerMask = 1 << 9; // Player 레이어 마스크
    private int subLayerMask = 1 << 8; // Door 레이어 마스크

    public List<Vector3> pathList;
    public bool drawPathLines = false;

    public class Waypoint
    {
        public float af;   // g + h
        public float ag;   // 시작위치부터 현재위치까지의 거리
        public float ah;   // 현재위치부터 목적지위치까지의 거리
        public int ID; // 웨이포인트 식별자
        public Vector3 position;
        public Waypoint parent;  // 부모 웨이포인트
        
        public Waypoint()
        {
            this.af = 0.0f;
            this.ag = 0.0f;
            this.ah = 0.0f;
            this.ID = 0;
            this.parent = null;
        }

        public Waypoint(Vector3 pos)
        {
            this.af = 0.0f;
            this.ag = 0.0f;
            this.ah = 0.0f;
            this.ID = 0;
            this.parent = null;
            this.position = pos;
        }
    }

    public int GetWPCount()
    {
        return waypointList.Count;
    }
    
    void Awake(){ 
        waypointList = new List<Waypoint>();
        layerMask |= subLayerMask; // 두가지를 합친다.
        layerMask = ~layerMask; // 반전시켜서 이 두가지만 무시하고 나머지 충돌체들은 무시하지 못하게 한다.

        // Waypoint 태그를 가진 모든 게임 오브젝트를 찾고 waypointList에 추가한다.
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject obj in waypoints)
        {
            Waypoint wp = new Waypoint(obj.transform.position);
            wp.ID = waypointID++;
            waypointList.Add(wp);
        }
    }
    
    public int FindPath(Vector3 start, Vector3 dest)
    {
        int resultPath = -1;
        
        // 각 리스트들을 초기화한다.
        openList = new List<Waypoint>();
        closedList = new List<Waypoint>();
        pathList = new List<Vector3>();

        foreach (Waypoint node in waypointList)
        {
            node.parent = null;
        }
        
        // 길찾을 필요없이 시작점에서 목적지까지 곧바로 갈 수 있는가?
        Vector3 Dir = dest - start;
        Dir.Normalize();
        if ((Physics.Raycast(start, Dir, Vector3.Distance(start, dest), layerMask)) == false)
        {
            // Add start location
            pathList.Add(start);
            // Add destination location
            pathList.Add(dest);
            // Path is found
            resultPath = 1;
            return resultPath;
        }
        // 시작위치를 열린리스트에 넣는다.
        Waypoint startLoc = new Waypoint(start);
        startLoc.af = startLoc.ah = Vector3.Distance(start, dest);
        openList.Add(startLoc);

        // 열린리스트에 웨이포인트가 남아있는 동안 반복한다.
        while (openList.Count != 0)
        {
            // 열린리스트에서 f값이 가장 작은 웨이포인트를 찾아내서 현재노드로 만든다.
            float f = openList[0].af;
            int index = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].af < f)
                {
                    f = openList[i].af;
                    index = i;
                }
            }
            Waypoint curWP = openList[index];

            // 현재노드에서 목적지로 바로 갈 수 있는가?
            Vector3 dirDest = dest - curWP.position;
            dirDest.Normalize();
            if ((Physics.Raycast(curWP.position, dirDest, Vector3.Distance(curWP.position, dest), layerMask)) == false)
            {
                pathList.Add(dest);
                CalculatePath(curWP);
                resultPath = 1;
                return resultPath;
            }

            // 현재노드의 자식노드들을 열린리스트에 넣는다.
            foreach (Waypoint childWP in waypointList)
            {
                int skip = 0;
                // 자기 자신인 노드는 건너뛴다.
                if (curWP.ID == childWP.ID)
                {
                    continue;
                }

                // 현재노드에서 자식노드로 레이캐스트를 발사한다.
                Vector3 dir = childWP.position - curWP.position;
                dir.Normalize();
                if ((Physics.Raycast(curWP.position, dir, Vector3.Distance(curWP.position, childWP.position), layerMask)) == false)
                {
                    // 이미 닫힌 리스트에 있다면 패스한다.
                    foreach (Waypoint closedWP in closedList)
                    {
                        if (childWP.ID == closedWP.ID)
                        {
                            skip = 1;
                            break;
                        }
                    }
                    if (skip == 1)
                    {
                        continue;
                    }
                    
                    // 이미 열린리스트에 있다면 g값을 비교해서 만약 현재노드를 거쳐서 가는 g값이 더 작다면 자식노드의 값들을 갱신한다.
                    foreach (Waypoint openWP in openList)
                    {
                        if (childWP.ID == openWP.ID)
                        {
                            if ((curWP.ag + Vector3.Distance(curWP.position, childWP.position)) < childWP.ag)
                            {
                                childWP.parent = curWP;
                                childWP.ag = curWP.ag + Vector3.Distance(curWP.position, childWP.position);
                                childWP.af = childWP.ag + childWP.ah;
                            }
                            skip = 1;
                            break;
                        }
                    }
                    if (skip == 1)
                    {
                        continue;
                    }

                    // 나머지 자식노드로 삼을 수 있는 노드의 초기값들을 설정해주고 현재노드를 부모노드로 하여 열린리스트에 넣는다.
                    childWP.ag = curWP.ag + Vector3.Distance(curWP.position, childWP.position);
                    childWP.ah = Vector3.Distance(childWP.position, dest);
                    childWP.af = childWP.ag + childWP.ah;
                    childWP.parent = curWP;
                    openList.Add(childWP);
                }
            }

            // 현재노드를 열린리스트에서 제거하고, 닫힌리스트에 추가한다.
            openList.RemoveAt(index);
            closedList.Add(curWP);
        }
        // 만약 경로를 못찾았으면 0 반환
        resultPath = 0;
        return resultPath;
    }

    private void CalculatePath(Waypoint node) {
        while (node != null)
        {
            pathList.Insert(0, node.position);
            node = node.parent;
        }
    }

    void OnDrawGizmos()
    {
        if ((pathList.Count != 0) && drawPathLines == true)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                Gizmos.DrawLine(pathList[i], pathList[i + 1]);
            }
        }
    }
}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNPC : MonoBehaviour {
    public GameObject thePath;
    public float moveSpeed = 1.0f;
    private WaypointMaster wpMaster;
    private CharacterController ccontroller;
    private Vector3 startPos, endPos;
    private float elapsedTime = 0.0f; // 경과 시간
    public float intervalTime = 1.0f; // 경로 탐색 사이의 시간 간격
    private Vector3 next, goal, moveDirection, curPosition;
    private int resultPath;
    private int idx;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        wpMaster = thePath.GetComponent<WaypointMaster>();
        ccontroller = GetComponent<CharacterController>();
        startPos = transform.position;
        goal = endPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        
        resultPath = wpMaster.FindPath(startPos, endPos);

        switch (resultPath)
        {
            case 0: print("can't find path");
                break;
            case 1:
                idx = wpMaster.pathList.Count > 1 ? 1 : 0;
                next = wpMaster.pathList[idx++];
                moveDirection = next - transform.position;
                break;
            default: break;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        curPosition = transform.position;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            
            resultPath = wpMaster.FindPath(transform.position, goal);

            switch (resultPath)
            {
                case 0:
                    print("can't find path");
                    break;
                case 1:
                    idx = wpMaster.pathList.Count > 1 ? 1 : 0;
                    next = wpMaster.pathList[idx++];
                    moveDirection = next - transform.position;
                    break;
                default: break;
            }
        }

        if (!(Vector3.Distance(transform.position, goal) < 0.4f) && resultPath != 0)
        {
            if (Vector3.Distance(transform.position, next) < 0.4f && idx < wpMaster.pathList.Count)
            {
                next = wpMaster.pathList[idx++];
                moveDirection = next - transform.position;
            }
            
            moveDirection.y = 0;
            Vector3 targetPos = transform.position + moveDirection;
            Vector3 framePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            Vector3 frameDir = framePos - transform.position;
            transform.rotation = Quaternion.LookRotation(frameDir);
            //frameDir.y -= gravity * Time.deltaTime;
            ccontroller.Move(frameDir + Physics.gravity);
        }
        
        if (curPosition != transform.position)
            anim.SetBool("IsWalking", true);
        else
            anim.SetBool("IsWalking", false);
    }
}
*/