using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] wallVariants; // Варианты стен для разнообразия
    public GameObject[] floorVariants; // Варианты пола
    public GameObject startPlatform; // Стартовая платформа
    public GameObject finishPlatform; // Финишная платформа

    [Header("Maze Settings")]
    public int width = 20;
    public int height = 20;
    public float cellSize = 3f;
    public float wallHeight = 3f;
    [Range(0f, 1f)] public float floorVariantChance = 0.2f; // Шанс использования альтернативного варианта пола

    private MazeCell[,] mazeCells;
    private Vector3 startPosition;
    private Vector3 finishPosition; // Позиция финиша
    private HashSet<Vector2Int> specialCells; // Для хранения позиций старта и финиша

    void Start()
    {
        GenerateMaze();
    }

    public void GenerateMaze()
    {
        // Очищаем предыдущий лабиринт
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Инициализация ячеек
        InitializeCells();

        // Инициализация множества специальных клеток
        specialCells = new HashSet<Vector2Int>();

        // Создаем лабиринт используя алгоритм Recursive Backtracking
        GenerateMazeRecursive(0, 0);

        // Создаем стартовую и финишную позиции
        CreateStartPosition();
        CreateFinishPosition();

        // Создаем визуальные элементы (после создания старта/финиша)
        CreateMazeVisuals();
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

        // Создаем список возможных направлений
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),  // Вправо
            new Vector2Int(-1, 0), // Влево
            new Vector2Int(0, 1),   // Вверх
            new Vector2Int(0, -1)   // Вниз
        };

        // Перемешиваем направления
        ShuffleDirections(directions);

        foreach (var dir in directions)
        {
            int newX = x + dir.x;
            int newZ = z + dir.y;

            // Проверяем, что новая ячейка в пределах лабиринта и не посещена
            if (newX >= 0 && newX < width && newZ >= 0 && newZ < height && !mazeCells[newX, newZ].visited)
            {
                // Убираем стену между текущей и новой ячейкой
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

                // Рекурсивно продолжаем из новой ячейки
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
                // Пропускаем создание пола для специальных клеток (старта и финиша)
                if (specialCells.Contains(new Vector2Int(x, z)))
                    continue;

                Vector3 cellPosition = new Vector3(x * cellSize, 0, z * cellSize);

                // Создаем пол с вариативностью
                CreateFloor(cellPosition);

                // Создаем стены
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
        if (floorToInstantiate != null)
        {
            Instantiate(floorToInstantiate, position, Quaternion.identity, transform);
        }
    }

    private GameObject GetRandomFloorVariant()
    {
        if (floorVariants.Length == 0) return null;

        // С шансом floorVariantChance выбираем случайный вариант, иначе первый в списке
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
    }

    private GameObject GetRandomWallVariant()
    {
        if (wallVariants.Length == 0) return null;
        return wallVariants[Random.Range(0, wallVariants.Length)];
    }

    private void CreateStartPosition()
    {
        // Стартовая позиция в левом нижнем углу (ячейка 0,0)
        startPosition = new Vector3(0, 0, 0);
        Instantiate(startPlatform, startPosition, Quaternion.identity, transform);
        specialCells.Add(new Vector2Int(0, 0)); // Добавляем в специальные клетки
    }

    private void CreateFinishPosition()
    {
        // Финишная позиция в правом верхнем углу
        int finishX = width - 1;
        int finishZ = height - 1;
        finishPosition = new Vector3(finishX * cellSize, 0, finishZ * cellSize);
        Instantiate(finishPlatform, finishPosition, Quaternion.identity, transform);
        specialCells.Add(new Vector2Int(finishX, finishZ)); // Добавляем в специальные клетки

        // Можно добавить специальные эффекты или триггер для финиша
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

// Класс для обработки достижения финиша
public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the finish!");
            // Здесь можно добавить логику завершения уровня:
            // - Показать сообщение о победе
            // - Загрузить следующий уровень
            // - Воспроизвести звук победы и т.д.

            // Пример простого сообщения:
            GameObject.FindFirstObjectByType<MazeGenerator>().SendMessage("OnFinishReached", SendMessageOptions.DontRequireReceiver);
        }
    }
}