using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    public GameObject grenadePrefab; 
    public int maxGrenades = 3; 
    public int currentGrenades;
    private Camera playerCamera; 


    void Start()
    {
        currentGrenades = maxGrenades;
        playerCamera = Camera.main;
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

        Vector3 throwDirection = playerCamera.transform.forward;

        GameObject grenade = Instantiate(grenadePrefab, transform.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {

            rb.AddForce(throwDirection * 10f, ForceMode.Impulse);
        }
    }
}
