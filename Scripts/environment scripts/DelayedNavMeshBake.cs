using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DelayedNavMeshBake : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;
    public float delayTime = 5f; 
    private void Start()
    {
        if (navMeshSurface == null)
        {
           
            navMeshSurface = GetComponent<NavMeshSurface>();

            if (navMeshSurface == null)
            {
                Debug.LogError("NavMeshSurface component not found!");
                return;
            }
        }

        StartCoroutine(BakeAfterDelay());
    }

    private System.Collections.IEnumerator BakeAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh baked after " + delayTime + " seconds");
    }
}