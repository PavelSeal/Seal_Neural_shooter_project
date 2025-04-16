using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab; // префаб гранаты
    public int maxGrenades = 3; // Максимальное количество гранат
    private int currentGrenades;
    private Camera playerCamera; // Ссылка на камеру игрока

    void Start()
    {
        currentGrenades = maxGrenades;
        playerCamera = Camera.main; // Получаем главную камеру игрока
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentGrenades > 0)
        {
            ThrowGrenade();
            currentGrenades--;
        }
    }

    void ThrowGrenade()
    {
        // Получаем направление взгляда камеры игрока
        Vector3 throwDirection = playerCamera.transform.forward;

        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Применяем направление взгляда к скорости броска
            rb.AddForce(throwDirection * 10f, ForceMode.Impulse);
        }
    }
}
