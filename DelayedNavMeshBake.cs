using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DelayedNavMeshBake : MonoBehaviour
{
    public NavMeshSurface navMeshSurface; // Ссылка на компонент NavMeshSurface
    public float delayTime = 5f; // Время задержки перед запеканием

    private void Start()
    {
        if (navMeshSurface == null)
        {
            // Попробуем получить компонент автоматически, если ссылка не задана
            navMeshSurface = GetComponent<NavMeshSurface>();

            if (navMeshSurface == null)
            {
                Debug.LogError("NavMeshSurface component not found!");
                return;
            }
        }

        // Запускаем корутину с задержкой
        StartCoroutine(BakeAfterDelay());
    }

    private System.Collections.IEnumerator BakeAfterDelay()
    {
        // Ждем указанное количество секунд
        yield return new WaitForSeconds(delayTime);

        // Запекаем NavMesh
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh baked after " + delayTime + " seconds");
    }
}