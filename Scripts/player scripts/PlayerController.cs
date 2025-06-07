using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; 
    public float mouseSensitivity = 100f; 
    public float crouchHeight = 1f; 
    public float runMultiplier = 1.5f; 
    public float jumpHeight = 2f; 
    public float gravity = -9.81f; 
    public float crouchTransitionSpeed = 5f; 
    public int maxHealth = 100;
    public int currentHealth; 

    public GameObject gameOverPanel; 

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity;
    private float originalHeight; 
    private bool isCrouching = false; 
    public Transform weaponHolder; 

    private Vector3 moveDirection; 
    private float targetHeight; 

    [Header("Footsteps Settings")]
    public AudioSource footstepAudioSource;
    public AudioClip footstepSound;
    public float walkStepInterval = 0.5f; 
    public float runStepInterval = 0.3f; 
    private float nextStepTime;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
        originalHeight = controller.height; 
        targetHeight = originalHeight; 
        currentHealth = maxHealth;

        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

     
        if (footstepAudioSource == null)
        {
            footstepAudioSource = gameObject.AddComponent<AudioSource>();
        }
        footstepAudioSource.clip = footstepSound;
        footstepAudioSource.loop = false;
        
    }

    void Update()
    {

        bool isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f) && controller.isGrounded;

        if (isMoving)
        {
            float currentStepInterval = Input.GetKey(KeyCode.LeftShift) ? runStepInterval : walkStepInterval;

            if (Time.time > nextStepTime)
            {
                PlayFootstepSound();
                nextStepTime = Time.time + currentStepInterval;
            }
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed * Time.deltaTime;

        if (isGrounded)
        {
            moveDirection = move;
        }

        controller.Move(moveDirection);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= runMultiplier;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= runMultiplier;
        }


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (weaponHolder != null)
        {
            weaponHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouching)
            {
                targetHeight = crouchHeight; 
                isCrouching = true;
            }
            else
            {
                targetHeight = originalHeight;
                isCrouching = false;
            }
        }

        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void PlayFootstepSound()
    {
        if (footstepSound != null && footstepAudioSource != null)
        {
            footstepAudioSource.pitch = Input.GetKey(KeyCode.LeftShift) ? 1.2f : 1.0f;
            footstepAudioSource.PlayOneShot(footstepSound);
        }
    }

    void Die()
    {
        Debug.Log("Игрок умер!");

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        this.enabled = false;
    }
}