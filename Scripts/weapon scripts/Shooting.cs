using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float bulletForce = 20f;

    public GameObject bulletPrefab;

    public float fireRate = 0.5f;

    public bool isAutomatic = false;

    public int maxAmmoInReserve = 100;

    public int currentAmmoInReserve;

    public int magazineSize = 30;

    public int currentAmmo;

    public float reloadTime = 2f;

    private bool isReloading = false;

    public Transform firePoint;

    private float nextFireTime = 0f;

    public float recoilBackForce = 0.1f;

    public float recoilSideForce = 0.05f;

    public float recoilRotationForce = 2f;

    public float recoilRecoverySpeed = 5f;

    private Vector3 originalPosition;

    private Quaternion originalRotation;

    public Camera playerCamera;

    public ParticleSystem bulletCreationParticles;
    public Light muzzleFlashLight;
    public float flashDuration = 0.05f;


    public AudioSource audioSource;
    public AudioClip shootSound;

    void Start()
    {
        currentAmmoInReserve = maxAmmoInReserve; 
        currentAmmo = magazineSize; 
        originalPosition = transform.localPosition; 
        originalRotation = transform.localRotation; 
    
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

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

        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoilRecoverySpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * recoilRecoverySpeed);

        if (Time.timeScale == 0) return;

        if (!isReloading && Time.time >= nextFireTime && currentAmmo > 0) 
        {
            if (isAutomatic)
            {
               
                if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    Shoot(); 
                    ApplyRecoil();
                    nextFireTime = Time.time + 1f / fireRate; 
                    currentAmmo--; 
                }
            }
            else
            {
                
                if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
                {
                    Shoot();
                    ApplyRecoil();
                    nextFireTime = Time.time + 1f / fireRate; 
                    currentAmmo--; 
                }
            } 
        }
        


        
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmoInReserve > 0)
        {
            StartCoroutine(Reload()); 
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("Не установлен FirePoint или BulletPrefab!");
            return;
        }

  
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
            Invoke(nameof(DisableMuzzleFlash), flashDuration); 
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
           targetPoint = hit.point;
        }
        else
        {
    
            targetPoint = ray.GetPoint(100);
        }

  
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;

    
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
        isReloading = true; 
        Debug.Log("Перезарядка началась...");

        float targetRotationX = 45f; 
        Quaternion startRotation = transform.localRotation; 
        Quaternion endRotation = Quaternion.Euler(targetRotationX, 0, 0) * startRotation; 

        float t = 0;
        while (t < reloadTime)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t / reloadTime * 4);
            t += Time.deltaTime;
            yield return null;
        }

     
        transform.localRotation = startRotation * Quaternion.Euler(targetRotationX, 0, 0);

        int ammoToLoad = magazineSize - currentAmmo; 
        if (ammoToLoad > currentAmmoInReserve)
        {
            ammoToLoad = currentAmmoInReserve; 
        }

        currentAmmo += ammoToLoad;
        currentAmmoInReserve -= ammoToLoad; 

        Debug.Log("Перезарядка завершена. В магазине: " + currentAmmo + ", В запасе: " + currentAmmoInReserve);
        isReloading = false; 
    }



    void ApplyRecoil()
    {
        Vector3 recoilOffset = new Vector3(
            Random.Range(-recoilSideForce, recoilSideForce),
            Random.Range(-recoilSideForce, recoilSideForce),
            -recoilBackForce
        );
        transform.localPosition += recoilOffset;

  
        Vector3 recoilRotation = new Vector3(
            Random.Range(-recoilRotationForce, recoilRotationForce),
            Random.Range(-recoilRotationForce, recoilRotationForce),
            0 
        );
        transform.localRotation *= Quaternion.Euler(recoilRotation);

    }

    private void DisableMuzzleFlash() 
    {
        muzzleFlashLight.enabled = false;
    }

    public void ResetWeaponState()
    {
        StopAllCoroutines(); 
        isReloading = false;
        transform.localPosition = originalPosition; 
        transform.localRotation = originalRotation;


        if (muzzleFlashLight != null && muzzleFlashLight.enabled)
        {
            muzzleFlashLight.enabled = false;
        }
    }
}
