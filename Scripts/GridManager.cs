using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour { // 싱글톤 인스턴스로 유지한다.
    private static GridManager s_Instance = null;

    public static GridManager instance
    { // 씬에서 GridManager 오브젝트를 찾아본 후 이미 있으면 이를 s_Instance static 변수에 할당해 관리한다.
        get
        {
            if(s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GridManager)) as GridManager;
                if (s_Instance == null)
                    Debug.Log("Could not locate a GridManager object. \n You have to have exactly one GridManager in the scene.");
            }
            return s_Instance;
        }
    }
    
    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;
    public bool showObstacleBlocks = true;

    private Vector3 origin;
    private GameObject[] obstacleList;
    public Node[,] nodes { get; set; }
    public Vector3 Origin
    {
        get { return origin; }
    }
    
    void Awake()
    {
        origin = GameObject.FindGameObjectWithTag("Grid").transform.position;
        obstacleList = GameObject.FindGameObjectsWithTag("Obstacle"); // Obstacle 태그를 가진 모든 게임 오브젝트를 찾고 obstacleList 속성에 추가한다.
        CalculateObstacles();
    }

    // 맵 상의 모든 장애물을 찾는다
    void CalculateObstacles()
    { 
        nodes = new Node[numOfColumns, numOfRows]; // 노드 2차원 배열 설정
        int index = 0;
        for (int i = 0; i < numOfColumns; i++)
        {
            for(int j = 0; j < numOfRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index); // 격자셀의 중심위치를 가져온다
                Node node = new Node(cellPos); // 그 위치의 노드를 생성
                nodes[i, j] = node; // 해당 위치의 배열에 넣는다
                index++;
            }
        }
        if(obstacleList != null && obstacleList.Length > 0) // 장애물 리스트가 비어있지 않고, null이 아니면
        { // 맵에서 발견한 각 장애물을 리스트에 기록한다
            foreach (GameObject data in obstacleList)
            {
                int indexCell = GetGridIndex(data.transform.position); // 각 장애물의 위치 인덱스를 얻는다
                int col = GetColumn(indexCell); // 해당 인덱스셀의 열을 얻는다
                int row = GetRow(indexCell); // 해당 인덱스셀의 행을 얻는다
                /*
                int x = (int)data.transform.localScale.x / (int)gridCellSize;
                int z = (int)data.transform.localScale.z / (int)gridCellSize;
                if(x % (int)gridCellSize == 0)
                {
                    x -= 1;
                }
                if(z % (int)gridCellSize == 0)
                {
                    z -= 1;
                }
                */
                nodes[row, col].MarkAsObstacle(); // 해당되는 노드를 장애물로 설정한다.
                
            }
        }
    }

    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize / 2.0f); // 설정한 격자셀의 크기를 2로 나눈 후 더해서 중심부의 위치로 이동한다
        cellPosition.z += (gridCellSize / 2.0f);
        return cellPosition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
    }

    public int GetGridIndex(Vector3 pos)
    {
        if (!IsInBounds(pos)) // 격자셀의 한도를 벗어나면 -2를 반환 (실패)
            return -2; 
        
        pos -= Origin;
        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);
        return (row * numOfColumns + col);
    }

    public bool IsInBounds(Vector3 pos) // 격자셀의 한도 안에 있는가?
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;
        return (pos.x >= Origin.x && pos.x <= Origin.x + width && pos.z <= Origin.z + height && pos.z >= Origin.z);
    }

    public int GetRow(int index) // 인덱스의 행을 구한다
    {
        int row = index / numOfColumns;
        return row;
    }

    public int GetColumn(int index) // 인덱스의 열을 구한다
    {
        int col = index % numOfColumns;
        return col;
    }

    public void GetNeighbours(Node node, ArrayList neighbors) // 열린 리스트에서 가져온 node와 이웃 노드를 저장할 neighbors
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column = GetColumn(neighborIndex);

        // 아래
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 위
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 오른쪽
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 왼쪽
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        // 오른쪽위
        if (neighborObstacle(row, column + 1) && neighborObstacle(row + 1, column)) // 대각선으로 벽을 뚫고 지나가지 않기 위해 사용
        {
            leftNodeRow = row + 1;
            leftNodeColumn = column + 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }

        // 왼쪽위
        if (neighborObstacle(row, column - 1) && neighborObstacle(row + 1, column))
        {
            leftNodeRow = row + 1;
            leftNodeColumn = column - 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }

        // 왼쪽아래
        if (neighborObstacle(row, column - 1) && neighborObstacle(row - 1, column))
        {
            leftNodeRow = row - 1;
            leftNodeColumn = column - 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }

        // 오른쪽아래
        if (neighborObstacle(row, column + 1) && neighborObstacle(row - 1, column))
        {
            leftNodeRow = row - 1;
            leftNodeColumn = column + 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }
    }

    void AssignNeighbour(int row, int column, ArrayList neighbors) // 이웃노드를 배정한다
    {
        if (row >= 0 && column >= 0 && row < numOfRows && column < numOfColumns) // 격자 한도 내에 있다면
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.bObstacle) // 장애물이 아니면
                neighbors.Add(nodeToAdd); // 이웃노드로 추가
        }
    }

    bool neighborObstacle(int row, int column) // 이웃노드가 장애물인지 확인한다.
    {
        if (row >= 0 && column >= 0 && row < numOfRows && column < numOfColumns) // 격자 한도 내에 있다면
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.bObstacle)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    void OnDrawGizmos() // 화면에 보이게 해주는 함수
    {
        if (showGrid)
        {
            DebugDrawGrid(transform.position, numOfRows, numOfColumns, gridCellSize, Color.blue);
        }
        Gizmos.DrawSphere(transform.position, 0.5f);
        if (showObstacleBlocks)
        {
            Vector3 cellSize = new Vector3(gridCellSize, 1.0f, gridCellSize);
            if(obstacleList != null && obstacleList.Length > 0)
            {
                foreach(GameObject data in obstacleList)
                {
                    Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(data.transform.position)), cellSize);
                }
            }
        }
    }

    public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numRows * cellSize);

        // 수평 격자 라인을 그린다
        for(int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startPos, endPos, color);
        }

        // 수직 격자 라인을 그린다
        for(int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }
}
