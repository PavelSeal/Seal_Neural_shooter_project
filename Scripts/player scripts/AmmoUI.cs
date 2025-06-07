using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public WeaponManager weaponManager; 
    public Text ammoText;

    private Shooting currentShooting;

    void Start()
    {
        UpdateCurrentWeapon();
    }

    void Update()
    {
        if (currentShooting != null && ammoText != null)
        {
            ammoText.text = $"{currentShooting.currentAmmo} / {currentShooting.currentAmmoInReserve}";
        }
    }

    public void UpdateCurrentWeapon()
    {
        if (weaponManager != null)
        {
            GameObject currentWeapon = weaponManager.GetCurrentWeapon();
            currentShooting = currentWeapon.GetComponent<Shooting>();
        }
    }
}