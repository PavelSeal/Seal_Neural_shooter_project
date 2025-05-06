using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]

    public GameObject[] wallVariants; // �������� ���� ��� ������������
    public GameObject[] floorVariants; // �������� ����
    public GameObject boundaryWallPrefab; // ������ ��� ��������� ����
    public GameObject startPlatform; // ��������� ���������
    public GameObject finishPlatform; // �������� ���������
    


    [Header("Maze Settings")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 3f;
    public float wallHeight = 3f;
    [Range(0f, 1f)] public float floorVariantChance = 0.2f; // ���� ������������� ��������������� �������� ����

    private MazeCell[,] mazeCells;
    private Vector3 startPosition;
    private Vector3 finishPosition; // ������� ������

    void Start()
    {
        GenerateMaze();
        
    }

    public void GenerateMaze()
    {
        // ������� ���������� ��������
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // ������������� �����
        InitializeCells();

        // ������� �������� ��������� �������� Recursive Backtracking
        GenerateMazeRecursive(0, 0);

        // ������� ���������� ��������
        CreateMazeVisuals();

        // ������� �������
        CreateBoundaryWalls();

        // ������� ��������� �������
        CreateStartPosition();

        CreateFinishPosition(); // ������� �������� �������
    }

    private void InitializeCells()
    {
        mazeCells = new MazeCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                mazeCells[x, z] = new MazeCell();
            }
        }
    }

    private void GenerateMazeRecursive(int x, int z)
    {
        mazeCells[x, z].visited = true;

        // ������� ������ ��������� �����������
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),  // ������
            new Vector2Int(-1, 0), // �����
            new Vector2Int(0, 1),   // �����
            new Vector2Int(0, -1)   // ����
        };

        // ������������ �����������
        ShuffleDirections(directions);

        foreach (var dir in directions)
        {
            int newX = x + dir.x;
            int newZ = z + dir.y;

            // ���������, ��� ����� ������ � �������� ��������� � �� ��������
            if (newX >= 0 && newX < width && newZ >= 0 && newZ < height && !mazeCells[newX, newZ].visited)
            {
                // ������� ����� ����� ������� � ����� �������
                if (dir.x == 1)
                {
                    mazeCells[x, z].rightWall = false;
                    mazeCells[newX, newZ].leftWall = false;
                }
                else if (dir.x == -1)
                {
                    mazeCells[x, z].leftWall = false;
                    mazeCells[newX, newZ].rightWall = false;
                }
                else if (dir.y == 1)
                {
                    mazeCells[x, z].topWall = false;
                    mazeCells[newX, newZ].bottomWall = false;
                }
                else if (dir.y == -1)
                {
                    mazeCells[x, z].bottomWall = false;
                    mazeCells[newX, newZ].topWall = false;
                }

                // ���������� ���������� �� ����� ������
                GenerateMazeRecursive(newX, newZ);
            }
        }
    }

    private void ShuffleDirections(List<Vector2Int> directions)
    {
        for (int i = 0; i < directions.Count; i++)
        {
            int randomIndex = Random.Range(i, directions.Count);
            Vector2Int temp = directions[randomIndex];
            directions[randomIndex] = directions[i];
            directions[i] = temp;
        }
    }

    private void CreateMazeVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);

                // ������� ��� � ��������������
                CreateFloor(cellPosition);

                // ������� �����
                MazeCell cell = mazeCells[x, z];
                float wallVerticalOffset = wallHeight / 2;

                if (cell.topWall)
                    CreateWall(
                        cellPosition + new Vector3(0, wallVerticalOffset, cellSize / 2),
                        Quaternion.identity);

                if (cell.bottomWall)
                    CreateWall(
                        cellPosition + new Vector3(0, wallVerticalOffset, -cellSize / 2),
                        Quaternion.identity);

                if (cell.leftWall)
                    CreateWall(
                        cellPosition + new Vector3(-cellSize / 2, wallVerticalOffset, 0),
                        Quaternion.Euler(0, 90, 0));

                if (cell.rightWall)
                    CreateWall(
                        cellPosition + new Vector3(cellSize / 2, wallVerticalOffset, 0),
                        Quaternion.Euler(0, 90, 0));
            }
        }
    }

    private void CreateFloor(Vector3 position)
    {
        GameObject floorToInstantiate = GetRandomFloorVariant();
        Instantiate(floorToInstantiate, position, Quaternion.identity, transform);
    }

    private GameObject GetRandomFloorVariant()
    {
        if (floorVariants.Length == 0) return null;

        // � ������ floorVariantChance �������� ��������� �������, ����� ������ � ������
        if (Random.value < floorVariantChance && floorVariants.Length > 1)
        {
            return floorVariants[Random.Range(1, floorVariants.Length)];
        }
        return floorVariants[0];
    }

    private void CreateWall(Vector3 position, Quaternion rotation)
    {
        GameObject wallToInstantiate = GetRandomWallVariant();
        if (wallToInstantiate == null) return;

        GameObject wall = Instantiate(wallToInstantiate, position, rotation, transform);
        // ������ ��������� �������� �����
    }

    private GameObject GetRandomWallVariant()
    {
        if (wallVariants.Length == 0) return null;
        return wallVariants[Random.Range(0, wallVariants.Length)];
    }


    private void CreateBoundaryWalls()
    {
        float halfCell = cellSize / 2;
        float totalWidth = width * cellSize;
        float totalHeight = height * cellSize;

        // ������� 4 ��������� ����� ������ ���������
        CreateBoundaryWall(new Vector3(totalWidth / 2 - halfCell, 0, -halfCell), new Vector3(totalWidth + cellSize, wallHeight, cellSize));
        CreateBoundaryWall(new Vector3(totalWidth / 2 - halfCell, 0, totalHeight - halfCell), new Vector3(totalWidth + cellSize, wallHeight, cellSize));
        CreateBoundaryWall(new Vector3(-halfCell, 0, totalHeight / 2 - halfCell), new Vector3(cellSize, wallHeight, totalHeight + cellSize));
        CreateBoundaryWall(new Vector3(totalWidth - halfCell, 0, totalHeight / 2 - halfCell), new Vector3(cellSize, wallHeight, totalHeight + cellSize));
    }

    private void CreateBoundaryWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(boundaryWallPrefab, position, Quaternion.identity, transform);
        // ������ ��������� �������� ��������� �����
    }

    private void CreateStartPosition()
    {
        // ��������� ������� � ����� ������ ����
        startPosition = new Vector3(0, 0, 0);
        Instantiate(startPlatform, startPosition, Quaternion.identity, transform);
    }

    private void CreateFinishPosition()
    {
        // �������� ������� � ������ ������� ����
        finishPosition = new Vector3((width - 1) * cellSize, 0, (height - 1) * cellSize);
        Instantiate(finishPlatform, finishPosition, Quaternion.identity, transform);

        // ����� �������� ����������� ������� ��� ������� ��� ������
        GameObject finishObj = GameObject.FindGameObjectWithTag("Finish");
        if (finishObj != null)
        {
            finishObj.AddComponent<FinishTrigger>();
        }
    }



    public Vector3 GetStartPosition()
    {
        return startPosition + new Vector3(cellSize / 2, 1, cellSize / 2);
    }

    public Vector3 GetFinishPosition()
    {
        return finishPosition + new Vector3(cellSize / 2, 0, cellSize / 2);
    }
}

public class MazeCell
{
    public bool visited = false;
    public bool topWall = true;
    public bool bottomWall = true;
    public bool leftWall = true;
    public bool rightWall = true;
}

// ����� ��� ��������� ���������� ������
public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the finish!");
            // ����� ����� �������� ������ ���������� ������:
            // - �������� ��������� � ������
            // - ��������� ��������� �������
            // - ������������� ���� ������ � �.�.

            // ������ �������� ���������:
            GameObject.FindFirstObjectByType<MazeGenerator>().SendMessage("OnFinishReached", SendMessageOptions.DontRequireReceiver);
        }
    }
}