using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private AmmoUI ammoUI;

    private int currentWeaponIndex = 0;
    private bool isSwitching = false;

    private void Start()
    {
        ValidateWeaponsArray();
        InitializeWeapons();
    }

    private void Update()
    {
        if (isSwitching || weapons.Length <= 1) return;

        HandleWeaponSwitchInput();
    }

    private void ValidateWeaponsArray()
    {
        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogError("Weapons array is not assigned or empty!", this);
            enabled = false;
        }
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                Debug.LogError($"Weapon at index {i} is null!", this);
                continue;
            }

            weapons[i].SetActive(i == currentWeaponIndex);

            if (i == currentWeaponIndex)
            {
                ResetWeaponState(weapons[i]);
            }
        }
    }

    private void HandleWeaponSwitchInput()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollDelta) > Mathf.Epsilon)
        {
            int direction = scrollDelta > 0 ? 1 : -1;
            int newIndex = (currentWeaponIndex + direction + weapons.Length) % weapons.Length;

            if (newIndex != currentWeaponIndex)
            {
                SwitchWeapon(newIndex);
            }
        }
    }

    private void SwitchWeapon(int newIndex)
    {
        isSwitching = true;

        // Деактивируем текущее оружие
        weapons[currentWeaponIndex].SetActive(false);

        // Активируем новое оружие
        weapons[newIndex].SetActive(true);
        ResetWeaponState(weapons[newIndex]);

        currentWeaponIndex = newIndex;
        isSwitching = false;

        UpdateAmmoUI();
    }

    private void ResetWeaponState(GameObject weapon)
    {
        Shooting shootingComponent = weapon.GetComponent<Shooting>();
        shootingComponent?.ResetWeaponState();
    }

    private void UpdateAmmoUI()
    {
        ammoUI?.UpdateCurrentWeapon();
    }

    public GameObject GetCurrentWeapon()
    {
        return weapons.Length > currentWeaponIndex ? weapons[currentWeaponIndex] : null;
    }
}