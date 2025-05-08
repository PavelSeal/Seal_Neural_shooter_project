using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;
    private Vector3 originalRotation;
    private bool isSwitching = false;
    public AmmoUI ammoUI;

    void Start()
    {
        // Деактивируем все оружия, кроме первого
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }

        // Сохраняем исходное вращение первого оружия
        originalRotation = weapons[currentWeaponIndex].transform.localEulerAngles;
    }

    void Update()
    {
        if (isSwitching) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int newIndex = (currentWeaponIndex + (scroll > 0 ? 1 : -1) + weapons.Length) % weapons.Length;
            if (newIndex != currentWeaponIndex)
            {
                StartCoroutine(SwitchWeaponCoroutine(newIndex));
            }
        }
    }

    IEnumerator SwitchWeaponCoroutine(int newIndex)
    {
        isSwitching = true;

        // 1. Наклон текущего оружия вниз
        GameObject currentWeapon = weapons[currentWeaponIndex];
        yield return StartCoroutine(RotateWeapon(currentWeapon, originalRotation, new Vector3(90f, 0f, 0f)));

        // 2. Деактивируем текущее оружие
        currentWeapon.SetActive(false);

        // 3. Активируем новое оружие (пока еще наклоненное)
        GameObject newWeapon = weapons[newIndex];
        newWeapon.SetActive(true);
        newWeapon.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

        // 4. Возврат наклона нового оружия
        yield return StartCoroutine(RotateWeapon(newWeapon, new Vector3(90f, 0f, 0f), originalRotation));

        // Обновляем текущий индекс
        currentWeaponIndex = newIndex;
        isSwitching = false;

        if (ammoUI != null)
        {
            ammoUI.UpdateCurrentWeapon();
        }
    }

    private IEnumerator RotateWeapon(GameObject weapon, Vector3 fromRotation, Vector3 toRotation)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            weapon.transform.localEulerAngles = Vector3.Lerp(fromRotation, toRotation, t);
            yield return null;
        }
    }

    public GameObject GetCurrentWeapon()
    {
        return weapons[currentWeaponIndex];
    }



}