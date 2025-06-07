using UnityEngine;

public class GrenadeBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            GrenadeThrower grenadeThrower = other.GetComponentInChildren<GrenadeThrower>();

            if (grenadeThrower.currentGrenades < grenadeThrower.maxGrenades)
            {
                grenadeThrower.currentGrenades = grenadeThrower.maxGrenades;
                Destroy(gameObject);
            }
        }
    }
}