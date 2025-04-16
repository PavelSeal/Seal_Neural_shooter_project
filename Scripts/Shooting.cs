using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float bulletForce = 20f; // ���� ��������
    public GameObject bulletPrefab; // ������ ����
    public float fireRate = 0.5f; // ���������������� (��������� � �������)
    public bool isAutomatic = false; // ����� ��������: true - ��������������, false - ������
    public int magazineSize = 10; // ���������� �������� � ��������

    public Transform firePoint; // ����� ��������
    private float nextFireTime = 0f; // ����� ���������� ��������
    private int currentAmmo; // ������� ���������� ��������

    // ��������� ������
    public float recoilBackForce = 0.1f; // ���� ������ �����
    public float recoilSideForce = 0.05f; // ���� ������ � �������
    public float recoilRotationForce = 2f; // ���� �������� ��� ������
    public float recoilRecoverySpeed = 5f; // �������� �������������� ����� ������

    private Vector3 originalPosition; // �������� ��������� ������
    private Quaternion originalRotation; // �������� �������� ������

    public Camera playerCamera; // ������ ������, ����� �������� � ����� ������

    void Start()
    {
        currentAmmo = magazineSize; // �������������� ���������� ��������
        originalPosition = transform.localPosition; // ��������� �������� ��������� ������
        originalRotation = transform.localRotation; // ��������� �������� �������� ������

        // ���� ������ �� ���������, ���������� �������� ������
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // �������������� ��������� � �������� ������ ����� ������
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoilRecoverySpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * recoilRecoverySpeed);

        // ��������� ����� �������� � ������� ��������
        if (isAutomatic)
        {
            // �������������� ��������
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0) // ����� ������ ���� ������ � ���� �������
            {
                Shoot();
                ApplyRecoil(); // ��������� ������
                nextFireTime = Time.time + 1f / fireRate; // ������������� ����� ���������� ��������
                currentAmmo--; // ��������� ���������� ��������
            }
        }
        else
        {
            // ������ ��������
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0) // ����� ������ ���� ������ � ���� �������
            {
                Shoot();
                ApplyRecoil(); // ��������� ������
                nextFireTime = Time.time + 1f / fireRate; // ������������� ����� ���������� ��������
                currentAmmo--; // ��������� ���������� ��������
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("FirePoint ��� BulletPrefab �� ���������!");
            return;
        }

        // ���������� �����, ���� �������� (����� ������)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // ��� �� ������ ������
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // ���� ��� ����� � ������, �������� � ����� ���������
            targetPoint = hit.point;
        }
        else
        {
            // ���� ��� �� ����� ������, �������� � ����� �� ���������� 100 ������
            targetPoint = ray.GetPoint(100);
        }

        // ����������� ��������
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

        // ������� ����
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // ������� ���� �������� � ����������� ����
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletForce;
        }
        else
        {
            Debug.LogWarning("� ���� ����������� ��������� Rigidbody!");
        }
    }

    void ApplyRecoil()
    {
        // �������� ������ ����� � � �������
        Vector3 recoilOffset = new Vector3(
            Random.Range(-recoilSideForce, recoilSideForce), // ��������� �������� � �������
            Random.Range(-recoilSideForce, recoilSideForce), // ��������� �������� �����/����
            -recoilBackForce // �������� �����
        );
        transform.localPosition += recoilOffset;

        // �������� ������ ��� ������� "��������"
        Vector3 recoilRotation = new Vector3(
            Random.Range(-recoilRotationForce, recoilRotationForce), // ��������� �������� �� ��� X
            Random.Range(-recoilRotationForce, recoilRotationForce), // ��������� �������� �� ��� Y
            0 // �������� �� ��� Z �� ���������
        );
        transform.localRotation *= Quaternion.Euler(recoilRotation);
    }
}