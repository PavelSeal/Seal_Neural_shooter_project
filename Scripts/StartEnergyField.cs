using UnityEngine;
using System.Collections;

public class StartEnergyField : MonoBehaviour
{
    public float delay = 5f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

