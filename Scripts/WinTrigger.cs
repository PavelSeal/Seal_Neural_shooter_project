using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class WinTrigger : MonoBehaviour
{
    public GameObject winScreen;
    public string playerTag = "Player";
    public float winDistance = 2f;

    [Header("Timer Settings")]
    public Text timerText;
    public bool showMilliseconds = true;
    public Text bestTimeText; 

    private GameObject currentWinScreen;
    private float levelStartTime;
    private bool timerRunning = true;
    private string currentSceneName;

    void Start()
    {

        if (winScreen != null && winScreen.activeSelf)
        {
            winScreen.SetActive(false);
        }

        currentSceneName = SceneManager.GetActiveScene().name;
        levelStartTime = Time.time;
        UpdateTimerDisplay();
        LoadBestTime(); // Загружаем рекорд при старте
    }

    void Update()
    {
        if (timerRunning)
        {
            UpdateTimerDisplay();

            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= winDistance && currentWinScreen == null)
            {
                WinGame();
            }
        }
    }

    void LoadBestTime()
    {
        if (bestTimeText != null)
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime_" + currentSceneName, -1f);
            if (bestTime > 0)
            {
                bestTimeText.text = "Рекорд: " + FormatTime(bestTime);
            }
            else
            {
                bestTimeText.text = "Рекорд: --:--";
            }
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            float timeElapsed = Time.time - levelStartTime;
            timerText.text = showMilliseconds ?
                FormatTimeWithMilliseconds(timeElapsed) :
                FormatTime(timeElapsed);
        }
    }

    string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    string FormatTimeWithMilliseconds(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}:{2:D3}",
                           timeSpan.Minutes,
                           timeSpan.Seconds,
                           timeSpan.Milliseconds);
    }

    void WinGame()
    {
        timerRunning = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentWinScreen = Instantiate(winScreen);
        currentWinScreen.SetActive(true); 

        currentWinScreen = Instantiate(winScreen);

        float finalTime = Time.time - levelStartTime;
        float bestTime = PlayerPrefs.GetFloat("BestTime_" + currentSceneName, float.MaxValue);

        bool isNewRecord = finalTime < bestTime;

     
        if (isNewRecord)
        {
            PlayerPrefs.SetFloat("BestTime_" + currentSceneName, finalTime);
            PlayerPrefs.Save(); 
        }

        Text[] winTexts = currentWinScreen.GetComponentsInChildren<Text>(true);

        foreach (Text textElement in winTexts)
        {
            if (textElement.name == "FinalTimeText")
            {
                textElement.text = "Ваше время: " +
                    (showMilliseconds ?
                     FormatTimeWithMilliseconds(finalTime) :
                     FormatTime(finalTime));
            }
            else if (textElement.name == "RecordText")
            {
                if (isNewRecord)
                {
                    textElement.text = "Новый рекорд!";
                    textElement.color = Color.yellow;
                }
                else
                {
                    textElement.text = "Лучшее время: " +
                        (showMilliseconds ?
                         FormatTimeWithMilliseconds(bestTime) :
                         FormatTime(bestTime));
                }
            }
        }


    }


}