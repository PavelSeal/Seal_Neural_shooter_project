using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // ������� �������� ��������
    public float mouseSensitivity = 100f; // ���������������� ����
    public float crouchHeight = 1f; // ������ ��� ����������
    public float runMultiplier = 1.5f; // ��������� �������� ��� ����
    public float jumpHeight = 2f; // ������ ������
    public float gravity = -9.81f; // ����������
    public float crouchTransitionSpeed = 5f; // �������� �������� ����� ����������� � ������������

    public int maxHealth = 100; // ������������ �������� ������
    private int currentHealth; // ������� �������� ������

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float originalHeight; // ����������� ������ ���������
    private bool isCrouching = false; // ���� ����������
    public Transform weaponHolder; // ������ �� ������ WeaponHolder

    private Vector3 moveDirection; // ����������� ��������
    private float targetHeight; // ������� ������ ���������

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // �������� ������
        originalHeight = controller.height; // ��������� ����������� ������
        targetHeight = originalHeight; // ������������� ������� ������
        currentHealth = maxHealth; // ������������� �������� ��������
    }

    void Update()
    {
        // ��������, ��������� �� �������� �� �����
        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ��������� ����, ����� �������� "��������" � �����
        }

        // ��������
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // ������������ ������� ��������
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed * Time.deltaTime;

        // ���� �������� �� �����, ��������� ����������� ��������
        if (isGrounded)
        {
            moveDirection = move;
        }

        // ��������� ��������
        controller.Move(moveDirection);

        // ���
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= runMultiplier; // ����������� ��������
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= runMultiplier; // ���������� ������� ��������
        }

        // ������
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // ���������� ����������
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // �������� ������
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // ������������� �������� ������ � �������
        if (weaponHolder != null)
        {
            weaponHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // ����������
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
            {
                targetHeight = crouchHeight; // ������������� ������� ������ ��� ����������
                isCrouching = true;
            }
            else
            {
                targetHeight = originalHeight; // ������������� ������� ������ ��� �����������
                isCrouching = false;
            }
        }

        // ������� ��������� ������
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // ���������� �������� �� ����
        if (currentHealth <= 0)
        {
            Die(); // ����� ������ ������, ���� �������� ������ ��� ����� ����
        }
    }

    void Die()
    {
        Debug.Log("����� ����!"); // ����������� ������ ������
        // �������� ������ ��� �������� ��� ������ �������� ��� ������
    }
}