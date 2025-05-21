using UnityEngine;
using UnityEngine.UI; // Добавляем для работы с UI

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // Базовая скорость движения
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public float crouchHeight = 1f; // Высота при приседании
    public float runMultiplier = 1.5f; // Множитель скорости при беге
    public float jumpHeight = 2f; // Высота прыжка
    public float gravity = -9.81f; // Гравитация
    public float crouchTransitionSpeed = 5f; // Скорость перехода между приседанием и выпрямлением

    public int maxHealth = 100; // Максимальное здоровье игрока
    public int currentHealth; // Текущее здоровье игрока

    public GameObject gameOverPanel; // Ссылка на UI-панель поражения

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float originalHeight; // Изначальная высота персонажа
    private bool isCrouching = false; // Флаг приседания
    public Transform weaponHolder; // Ссылка на объект WeaponHolder

    private Vector3 moveDirection; // Направление движения
    private float targetHeight; // Целевая высота персонажа

    [Header("Footsteps Settings")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepSound;
    public float walkStepInterval = 0.5f; // Интервал между шагами при ходьбе
    public float runStepInterval = 0.3f; // Интервал между шагами при беге
    private float nextStepTime;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Скрываем курсор
        originalHeight = controller.height; // Сохраняем изначальную высоту
        targetHeight = originalHeight; // Устанавливаем целевую высоту
        currentHealth = maxHealth; // Инициализация текущего здоровья

        // Скрываем панель поражения при старте игры
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Инициализация звука шагов
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
        }
        footstepAudioSource.clip = footstepSound;
        footstepAudioSource.loop = false;
        
    }

    void Update()
    {
        // Проверка, находится ли персонаж на земле
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Небольшая сила, чтобы персонаж "прилипал" к земле
        }

        // Движение
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f) && controller.isGrounded;

        // Воспроизведение звуков шагов
        if (isMoving)
        {
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) ? runStepInterval : walkStepInterval;

            if (Time.time > nextStepTime)
            {
                PlayFootstepSound();
                nextStepTime = Time.time + currentStepInterval;
            }
        }

        // Нормализация вектора движения
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed * Time.deltaTime;

        // Если персонаж на земле, обновляем направление движения
        if (isGrounded)
        {
            moveDirection = move;
        }

        // Применяем движение
        controller.Move(moveDirection);

        // Бег
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= runMultiplier; // Увеличиваем скорость
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= runMultiplier; // Возвращаем базовую скорость
        }

        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Применение гравитации
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Вращение камеры
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Синхронизация вращения оружия с камерой
        if (weaponHolder != null)
        {
            weaponHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Приседание
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
            {
                targetHeight = crouchHeight; // Устанавливаем целевую высоту для приседания
                isCrouching = true;
            }
            else
            {
                targetHeight = originalHeight; // Устанавливаем целевую высоту для выпрямления
                isCrouching = false;
            }
        }

        // Плавное изменение высоты
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Чтобы здоровье не ушло в минус
            Die();
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSound != null && footstepAudioSource != null)
        {
            // Изменяем pitch для эффекта ускорения при беге
            footstepAudioSource.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.2f : 1.0f;
            footstepAudioSource.PlayOneShot(footstepSound);
        }
    }

    void Die()
    {
        Debug.Log("Игрок умер!");

        // Останавливаем игру
        Time.timeScale = 0f;

        // Показываем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Активируем панель поражения
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Отключаем управление игроком (опционально)
        this.enabled = false;
    }
}