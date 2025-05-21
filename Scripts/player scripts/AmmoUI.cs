using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public WeaponManager weaponManager; // —сылка на WeaponManager
    public Text ammoText; // —сылка на Text элемент

    private Shooting currentShooting;

    void Start()
    {
        // »нициализаци€ при старте
        UpdateCurrentWeapon();
    }

    void Update()
    {
        if (currentShooting != null && ammoText != null)
        {
            ammoText.text = $"{currentShooting.currentAmmo} / {currentShooting.currentAmmoInReserve}";
        }
    }

    // ћетод дл€ обновлени€ ссылки на текущее оружие
    public void UpdateCurrentWeapon()
    {
        if (weaponManager != null)
        {
            GameObject currentWeapon = weaponManager.GetCurrentWeapon();
            currentShooting = currentWeapon.GetComponent<Shooting>();
        }
    }
}