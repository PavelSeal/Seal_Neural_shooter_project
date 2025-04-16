using UnityEngine;

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
    private int currentHealth; // Текущее здоровье игрока

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float originalHeight; // Изначальная высота персонажа
    private bool isCrouching = false; // Флаг приседания
    public Transform weaponHolder; // Ссылка на объект WeaponHolder

    private Vector3 moveDirection; // Направление движения
    private float targetHeight; // Целевая высота персонажа

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Скрываем курсор
        originalHeight = controller.height; // Сохраняем изначальную высоту
        targetHeight = originalHeight; // Устанавливаем целевую высоту
        currentHealth = maxHealth; // Инициализация текущего здоровья
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
        currentHealth -= damage; // Уменьшение здоровья на урон
        if (currentHealth <= 0)
        {
            Die(); // Вызов метода смерти, если здоровье меньше или равно нулю
        }
    }

    void Die()
    {
        Debug.Log("Игрок умер!"); // Логирование смерти игрока
        // Добавьте логику для респауна или других действий при смерти
    }
}