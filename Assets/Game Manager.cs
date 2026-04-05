using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject startUI;
    public GameObject pauseUI;
    public GameObject restartHintUI;
    public TMP_Text timerText;

    [Header("Controls")]
    public KeyCode startKey = KeyCode.Space;
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Start Overlay")]
    [Range(0f, 1f)] public float startPanelMaxAlpha = 0.45f;

    private bool gameStarted = false;
    private float runTimer = 0f;

    private void Start()
    {
        Time.timeScale = 0f;
        ApplyStartOverlayAlpha();

        SetStartUIVisible(true);
        SetPauseUIVisible(false);
        SetRestartHintVisible(false);
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!gameStarted)
        {
            if (Input.GetKeyDown(startKey))
            {
                StartRun();
            }

            return;
        }

        if (Input.GetKeyDown(pauseKey) && Time.timeScale > 0f)
        {
            PauseRun();
        }
        else if (Input.GetKeyDown(pauseKey) && Time.timeScale == 0f && pauseUI != null && pauseUI.activeSelf)
        {
            ResumeRun();
        }

        if (Time.timeScale > 0f)
        {
            runTimer += Time.deltaTime;
            UpdateTimerUI();
        }

        SetRestartHintVisible(Time.timeScale == 0f);

        if (Time.timeScale == 0f && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void StartRun()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        SetStartUIVisible(false);
        SetPauseUIVisible(false);
    }

    private void PauseRun()
    {
        Time.timeScale = 0f;
        SetPauseUIVisible(true);
    }

    private void ResumeRun()
    {
        Time.timeScale = 1f;
        SetPauseUIVisible(false);
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {runTimer:0.0}s";
        }
    }

    private void SetStartUIVisible(bool visible)
    {
        if (startUI != null)
        {
            startUI.SetActive(visible);
        }
    }

    private void SetPauseUIVisible(bool visible)
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(visible);
        }
    }

    private void SetRestartHintVisible(bool visible)
    {
        if (restartHintUI != null)
        {
            restartHintUI.SetActive(visible);
        }
    }

    private void ApplyStartOverlayAlpha()
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