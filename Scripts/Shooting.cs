using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Сила выстрела пули
    public float bulletForce = 20f;

    // Префаб пули, который будет создаваться при выстреле
    public GameObject bulletPrefab;

    // Скорость стрельбы (выстрелы в секунду)
    public float fireRate = 0.5f;

    // Флаг, указывающий на автоматическую стрельбу
    public bool isAutomatic = false;

    // Максимальное количество патронов в запасе
    public int maxAmmoInReserve = 100;

    // Текущее количество патронов в запасе
    public int currentAmmoInReserve;

    // Размер магазина с пулями
    public int magazineSize = 30;

    // Текущее количество пуль в магазине
    public int currentAmmo;

    // Время перезарядки (в секундах)
    public float reloadTime = 2f;

    // Флаг, указывающий на перезарядку
    private bool isReloading = false;

    // Точка, из которой будет выпускаться пуля
    public Transform firePoint;

    // Время следующего допустимого выстрела
    private float nextFireTime = 0f;

    // Сила отдачи назад
    public float recoilBackForce = 0.1f;

    // Сила отдачи в стороны
    public float recoilSideForce = 0.05f;

    // Сила вращения при отдаче
    public float recoilRotationForce = 2f;

    // Скорость восстановления позиции после отдачи
    public float recoilRecoverySpeed = 5f;

    // Оригинальная позиция объекта
    private Vector3 originalPosition;

    // Оригинальное вращение объекта
    private Quaternion originalRotation;

    // Главная камера игрока
    public Camera playerCamera;

    // Система частиц для выстрела (дульная вспышка)
    public ParticleSystem bulletCreationParticles;
    public Light muzzleFlashLight;
    public float flashDuration = 0.05f;

    // Звук выстрела
    public AudioSource audioSource;
    public AudioClip shootSound;

    void Start()
    {
        currentAmmoInReserve = maxAmmoInReserve; // Инициализация количества патронов в запасе
        currentAmmo = magazineSize; // Инициализация текущего количества пуль в магазине
        originalPosition = transform.localPosition; // Сохранение оригинальной позиции объекта
        originalRotation = transform.localRotation; // Сохранение оригинального вращения объекта
    
        // Если камера не задана, назначаем основную камеру игрока
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Проверка и инициализация AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        // Линейное интерполирование позиции объекта к оригинальной позиции с учетом скорости восстановления отдачи
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoilRecoverySpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * recoilRecoverySpeed);

        if (!isReloading && Time.time >= nextFireTime && currentAmmo > 0) 
        {
            if (isAutomatic)// Если стрельба автоматическая
            {
                // Проверка нажатия кнопки "Fire1" и допустимого времени для выстрела и наличия патронов
                if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    Shoot(); // Вызов метода выстрела
                    ApplyRecoil(); // Применение отдачи
                    nextFireTime = Time.time + 1f / fireRate; // Обновление времени следующего выстрела
                    currentAmmo--; // Уменьшение количества патронов
                }
            }
            else
            {
                // Проверка нажатия кнопки "Fire1" и допустимого времени для выстрела и наличия патронов
                if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    Shoot(); // Вызов метода выстрела
                    ApplyRecoil(); // Применение отдачи
                    nextFireTime = Time.time + 1f / fireRate; // Обновление времени следующего выстрела
                    currentAmmo--; // Уменьшение количества патронов
                }
            } 
        }
        


        // Проверка нажатия кнопки "R" для перезарядки
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmoInReserve > 0)
        {
            StartCoroutine(Reload()); // Запуск процесса перезарядки
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("Не установлен FirePoint или BulletPrefab!");
            return;
        }

        // Воспроизведение звука выстрела
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (bulletCreationParticles != null)
        {
            bulletCreationParticles.Play();
        }

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.enabled = true;
            Invoke(nameof(DisableMuzzleFlash), flashDuration); // Выключить через flashDuration секунд
        }

        // Создание луча от центра экрана до ближайшей точки попадания
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Если луч попал в объект, используем точку попадания
            targetPoint = hit.point;
        }
        else
        {
            // Иначе используем дальнюю точку (100 единиц)
            targetPoint = ray.GetPoint(100);
        }

        // Направление выстрела
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // Создание пули с правильным вращением
        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletForce;
        }
        else
        {
            Debug.LogWarning("Пуля не имеет компонента Rigidbody!");
        }
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true; // Установка флага перезарядки
        Debug.Log("Перезарядка началась...");

        float targetRotationX = 45f; // Целевой угол поворота в градусах
        Quaternion startRotation = transform.localRotation; // Начальное вращение
        Quaternion endRotation = Quaternion.Euler(targetRotationX, 0, 0) * startRotation; // Вращение на 30 градусов по оси X

        float t = 0;
        while (t < reloadTime)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t / reloadTime * 4);
            t += Time.deltaTime;
            yield return null;
        }

        // Устанавливаем конечное вращение после перезарядки
        transform.localRotation = startRotation * Quaternion.Euler(targetRotationX, 0, 0);

        int ammoToLoad = magazineSize - currentAmmo; // Расчет количества патронов для загрузки
        if (ammoToLoad > currentAmmoInReserve)
        {
            ammoToLoad = currentAmmoInReserve; // Если требуется больше, чем есть в запасе, берем только доступное количество
        }

        currentAmmo += ammoToLoad; // Загрузка патронов в магазин
        currentAmmoInReserve -= ammoToLoad; // Уменьшение количества патронов в запасе

        Debug.Log("Перезарядка завершена. В магазине: " + currentAmmo + ", В запасе: " + currentAmmoInReserve);
        isReloading = false; // Сброс флага перезарядки
    }



    // отдача
    void ApplyRecoil()
    {
        // Случайное смещение позиции для имитации отдачи
        Vector3 recoilOffset = new Vector3(
            Random.Range(-recoilSideForce, recoilSideForce),
            Random.Range(-recoilSideForce, recoilSideForce),
            -recoilBackForce // Назад
        );
        transform.localPosition += recoilOffset;

        // Случайное вращение для имитации отдачи
        Vector3 recoilRotation = new Vector3(
            Random.Range(-recoilRotationForce, recoilRotationForce),
            Random.Range(-recoilRotationForce, recoilRotationForce),
            0 // Вращение только по осям X и Y
        );
        transform.localRotation *= Quaternion.Euler(recoilRotation);

    }

    private void DisableMuzzleFlash() //вспышки света
    {
        muzzleFlashLight.enabled = false;
    }

}
