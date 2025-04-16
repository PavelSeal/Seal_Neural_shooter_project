using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // ������ ������
    private int currentWeaponIndex = 0; // ������ �������� ������

    void Start()
    {
        // ���������� ������ ������ ��� ������
        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {
        // ������������ ������ �� ������ ����
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
        // ������������ ��� ������
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // ���������� ��������� ������
        weapons[index].SetActive(true);
    }
}