using UnityEngine;
using System.Collections;

public class StartEnergyField : MonoBehaviour
{
    public float delay = 5f;

    IEnumerator Start()
    {
        // Ждём delay секунд
        yield return new WaitForSeconds(delay);
        // Уничтожаем объект
        Destroy(gameObject);
    }
}

