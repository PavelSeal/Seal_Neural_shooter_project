using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    private bool isInMenu = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isInMenu)
        {
            ReturnToMainMenu();
        }

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

         Cursor.visible = false; 
        Cursor.lockState = CursorLockMode.Locked;
    }
}