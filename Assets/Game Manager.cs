using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject startUI;
    public GameObject restartHintUI;
    public TMP_Text timerText;
    public KeyCode startKey = KeyCode.Space;
    [Range(0f, 1f)] public float startPanelMaxAlpha = 0.45f;

    bool gameStarted = false;
    float runTimer = 0f;

    void Start()
    {
        ClampStartPanelAlpha();
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

    void ClampStartPanelAlpha()
    {
        if (startUI == null)
        {
            return;
        }

        Image panelImage = startUI.GetComponent<Image>();
        if (panelImage == null)
        {
            return;
        }

        Color color = panelImage.color;
        color.r = 0f;
        color.g = 0f;
        color.b = 0f;
        color.a = Mathf.Min(color.a, startPanelMaxAlpha);
        panelImage.color = color;
    }
}