using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // Массив оружий
    private int currentWeaponIndex = 0; // Индекс текущего оружия

    void Start()
    {
        // Активируем первое оружие при старте
        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // Переключение оружия по колесу мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            }
            else
            {
                currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            }
            SwitchWeapon(currentWeaponIndex);
        }
    }

    void SwitchWeapon(int index)
    {
        // Деактивируем все оружия
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // Активируем выбранное оружие
        weapons[index].SetActive(true);
    }
}