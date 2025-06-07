using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [Tooltip("������� ���������� ����������� (�� ������������� ������)")]
    [Range(0, 100)]
    public float ammoRefillPercent = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shooting playerShooting = other.GetComponentInChildren<Shooting>();

            if (playerShooting != null)
            {
                int ammoToAdd = Mathf.RoundToInt(playerShooting.maxAmmoInReserve * ammoRefillPercent / 100f);

                if (playerShooting.currentAmmoInReserve < playerShooting.maxAmmoInReserve)
                {
                    playerShooting.currentAmmoInReserve += ammoToAdd;

                    if (playerShooting.currentAmmoInReserve > playerShooting.maxAmmoInReserve)
                    {
                        playerShooting.currentAmmoInReserve = playerShooting.maxAmmoInReserve;
                    }

                    Debug.Log($"��������� {ammoToAdd} ��������. ������ � ������: {playerShooting.currentAmmoInReserve}");

                    Destroy(gameObject);
                }
            }
        }
    }
}