using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    private bool isInMenu = false;

    void Update()
    {
        // Возврат в меню по ESC
        if (Input.GetKeyDown(KeyCode.Escape) && !isInMenu)
        {
            ReturnToMainMenu();
        }

        // Показать курсор при любом нажатии в меню
        if (isInMenu && Input.anyKeyDown)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ReturnToMainMenu()
    {
        isInMenu = true;
        SceneManager.LoadScene(mainMenuScene);

        // Настройки курсора для меню
        Cursor.visible = false; // Сначала скрываем
        Cursor.lockState = CursorLockMode.Locked;
    }
}