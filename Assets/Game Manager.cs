using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public GameObject startUI;
    public GameObject restartHintUI;
    public Text timerText;
    public KeyCode startKey = KeyCode.Space;

    bool gameStarted = false;
    float runTimer = 0f;

    void Start()
    {
        Time.timeScale = 0f;
        ToggleStartUI(true);
        ToggleRestartHint(false);
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(startKey))
            {
                StartRun();
            }
            return;
        }

        if (Time.timeScale > 0f)
        {
            runTimer += Time.deltaTime;
            UpdateTimerUI();
        }

        ToggleRestartHint(Time.timeScale == 0f);

        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void StartRun()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        ToggleStartUI(false);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {runTimer:0.0}s";
        }
    }

    void ToggleStartUI(bool visible)
    {
        if (startUI != null)
        {
            startUI.SetActive(visible);
        }
    }

    void ToggleRestartHint(bool visible)
    {
        if (restartHintUI != null)
        {
            restartHintUI.SetActive(visible);
        }
    }
}