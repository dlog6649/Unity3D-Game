using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Behaviors { Idle, Guard, Chase }; // patrol, Seek

public class AI_Agent : MonoBehaviour {
    public bool isSuspicious = false;
    public bool isInRange = false;
    public Behaviors aiBehaviors = Behaviors.Guard;

    //public bool FightsRanged = false;
    //public List<KeyValuePair<string, int>> Stats = new List<KeyValuePair<string, int>>();
    //public GameObject Projectile;

    private int randomNum;
    private Animator anim;
    private MoveNPC moveNpc;
    private Vector3 endPos;
    private Vector3 playerLocation;
    private GameObject[] patrolNodes;
    private List<Vector3> patrolList = new List<Vector3>();
    private Transform playerTransform;
    private float elapsedTime = 0.0f;
    float intervalTime = 3.0f;
    int sum = 0;

    void Start()
    {
        moveNpc = GetComponent<MoveNPC>();
        anim = GetComponent<Animator>();
        //wpMasterList = thePaths.GetComponent<WaypointMaster>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerLocation = playerTransform.position;

        // 정찰다닐 노드 리스트 생성
        patrolNodes = GameObject.FindGameObjectsWithTag("PatrolNode");
        foreach (GameObject obj in patrolNodes)
        {
            Vector3 pl = obj.transform.position;
            patrolList.Add(pl);
        }
        randomNum = Random.Range(0, patrolList.Count - 1);
        
    }

    void Update()
    {
        RunBehaviors();
        
        switch (aiBehaviors)
        {
            case Behaviors.Idle:
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsRunning", false);
                break;
            case Behaviors.Guard:
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsRunning", false);
                break;
            case Behaviors.Chase:
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsRunning", true);
                break;
            default:
                break;
        }
    }

    public void SetPlayerLocation(Vector3 pos)
    {
        playerLocation = pos;
    }

    void RunBehaviors()
    {
        switch (aiBehaviors)
        {
            case Behaviors.Idle:
                RunIdleNode();
                break;
            case Behaviors.Guard:
                RunGuardNode();
                break;
            case Behaviors.Chase:
                RunChaseNode();
                break;
        }
    }

    public void ChangeBehavior(Behaviors newBehavior)
    {
        aiBehaviors = newBehavior;

        RunBehaviors();
    }

    void RunIdleNode()
    {
        Idle();
    }

    void RunGuardNode()
    {
        Guard();
    }

    void RunChaseNode()
    {
        Chase();
    }

    void Idle()
    {
        intervalTime = 3.0f;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            isSuspicious = false;
            ChangeBehavior(Behaviors.Guard);
        }
    }

    void Guard()
    {
        if (isSuspicious)
        {
            SearchForTarget();
        }
        else
        {
            Patrol();
        }
    }

    void Chase()
    {
        if (isInRange)
        {
            Attack();
        }
        else
        {
            ChaseTarget();
        }
    }

    void SearchForTarget() // 도중에 추격모드로 전환되는것에 대비해서 다른곳에서도 resultPath와 isSuspicious의 값을 변경해줄수잇어야함.
    {
        float distanceToDestination = Vector3.Distance(transform.position, playerLocation);

        if (distanceToDestination > 1.0f)
        {
            if(moveNpc.GoToLocation() == 0) // 길을 못찾아서 0을 반환했다면, 정찰모드로 전환
            {
                isSuspicious = false;
                moveNpc.SetResultPath(-1);
            }
        }
        else
        {
            if (sum < 135)
            {
                anim.SetBool("Lookaround", true);
                sum++;
                transform.Rotate(0, 1, 0);
            }
            else
            {
                sum++;
                transform.Rotate(0, -1, 0);
                if(sum >= 405)
                {
                    anim.SetBool("Lookaround", false);
                    sum = 0;
                    moveNpc.SetResultPath(-1);
                    isSuspicious = false;
                    ChangeBehavior(Behaviors.Guard);
                }
            }
        }
    }

    void ChaseTarget()
    {
        float distanceToDestination = Vector3.Distance(transform.position, playerTransform.position);
        isSuspicious = true;

        if(playerTransform.position.x >= 30 || playerTransform.position.x <= -30 || playerTransform.position.z >= 40 || playerTransform.position.z <= -40)
        {
            isSuspicious = false;
            moveNpc.SetResultPath(-1);
            ChangeBehavior(Behaviors.Idle);
        }

        // 일정 거리 내에 있으면 계속 추격
        if (distanceToDestination > 1.0f && distanceToDestination < 21.0f)
        {
            moveNpc.FollowTarget(playerTransform.position);
            isInRange = false;
        }// 가까워지면 근접공격
        else if(distanceToDestination <= 1.0f)
        {
            isInRange = true;
            moveNpc.SetResultPath(-1);
        }// 일정 거리 밖으로 벗어나면 가드모드로 전환.
        else
        {
            isInRange = false;
            moveNpc.SetResultPath(-1);
            playerLocation = playerTransform.position;
            ChangeBehavior(Behaviors.Guard);
        }
    }

    void Patrol()
    {
        // 목표하는 정찰노드에 도착했는지 검사한다.
        if (Vector3.Distance(patrolList[randomNum], transform.position) <= 1.0f)
        {
            randomNum = Random.Range(0, patrolList.Count - 1);
            moveNpc.SetResultPath(-1);
            //print(randomNum);
        }

        moveNpc.Patrol(patrolList[randomNum]);
    }

    void Attack()
    {
        playerLocation = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 lookPlayerDir = playerLocation - transform.position;
        lookPlayerDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPlayerDir);
        anim.SetTrigger("CanAttack");
    }

    /*
    void RangedAttack()
    {
        GameObject newProjectile;
        newProjectile = Instantiate(Projectile, transform.position, Quaternion.identity) as GameObject;
    }
    */
}
