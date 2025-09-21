using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Timer UI")]
    public TextMeshProUGUI timerText; 
    public Image timerFillBar;

    [Header("Warning UI")]
    public Color normalColor = Color.blue;
    public Color warningColor = Color.red;
    public bool enableBlinking = true;
    public float blinkSpeed = 2f;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;

    [Header("Additional UI Elements")]
    public GameObject warningIcon;

    [Header("Player Info UI")]
    public TextMeshProUGUI playerNameText;

    // Variables privadas
    private bool isWarningActive = false;
    private float blinkTimer = 0f;
    private bool isBlinking = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        TimeManager.OnTimeChanged += UpdateTimerDisplay;
        TimeManager.OnTimeWarning += ActivateWarning;
        TimeManager.OnTimeUp += OnTimeUp;

        ScoreManager.OnScoreChanged += UpdateScoreDisplay;

        InitializeUI();

        Debug.Log("UIManager inicializado con TMPro");
    }

    void InitializeUI()
    {
        if (timerText != null)
            timerText.color = normalColor;

        if (timerFillBar != null)
            timerFillBar.fillAmount = 1f;

        if (warningIcon != null)
            warningIcon.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            UpdateScoreDisplay(ScoreManager.Instance.GetCurrentScore());
        }

        if (playerNameText != null && AlmacenarNombre.Instance != null && AlmacenarNombre.Instance.HasPlayerName())
        {
            playerNameText.text = "Jugador: " + AlmacenarNombre.Instance.GetPlayerName();
        }
    }

    void Update()
    {
        if (isWarningActive && enableBlinking)
            HandleBlinking();
    }

    void UpdateTimerDisplay(float currentTime)
    {
        if (timerText != null)
        {
            string formattedTime = FormatTime(currentTime);
            timerText.text = formattedTime;
        }

        if (timerFillBar != null && TimeManager.Instance != null)
        {
            float percentage = currentTime / TimeManager.Instance.GetTotalTime();
            timerFillBar.fillAmount = percentage;
            timerFillBar.color = (percentage <= 0.25f) ? warningColor : normalColor;
        }
    }

    void ActivateWarning()
    {
        isWarningActive = true;

        if (warningIcon != null)
            warningIcon.SetActive(true);

        if (!enableBlinking && timerText != null)
            timerText.color = warningColor;

        Debug.Log("UI Warning activada");
    }

    void HandleBlinking()
    {
        if (timerText == null) return;

        blinkTimer += Time.deltaTime * blinkSpeed;

        if (blinkTimer >= 1f)
        {
            blinkTimer = 0f;
            isBlinking = !isBlinking;
            timerText.color = isBlinking ? warningColor : normalColor;
        }
    }

    void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore.ToString("N0");
    }

    public void ShowScoreAddition(int points)
    {
        if (scoreText != null)
            StartCoroutine(ShowScoreAdditionCoroutine(points));
    }

    System.Collections.IEnumerator ShowScoreAdditionCoroutine(int points)
    {
        string originalText = scoreText.text;
        Color originalColor = scoreText.color;

        scoreText.text = "+" + points.ToString();
        scoreText.color = Color.green;

        yield return new WaitForSeconds(0.5f);

        scoreText.text = originalText;
        scoreText.color = originalColor;
    }

    void OnTimeUp()
    {
        if (timerText != null)
        {
            timerText.text = "00:00";
            timerText.color = warningColor;
        }

        if (timerFillBar != null)
        {
            timerFillBar.fillAmount = 0f;
            timerFillBar.color = warningColor;
        }
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetTimerColor(Color color)
    {
        if (timerText != null)
            timerText.color = color;
    }

    public void SetWarningSettings(Color warning, Color normal, bool blink, float speed)
    {
        warningColor = warning;
        normalColor = normal;
        enableBlinking = blink;
        blinkSpeed = speed;
    }

    public void ShowTemporaryMessage(string message, float duration)
    {
        if (timerText != null)
            StartCoroutine(ShowMessageCoroutine(message, duration));
    }

    System.Collections.IEnumerator ShowMessageCoroutine(string message, float duration)
    {
        string originalText = timerText.text;
        timerText.text = message;

        yield return new WaitForSeconds(duration);

        if (TimeManager.Instance != null && TimeManager.Instance.GetCurrentTime() > 0f)
            timerText.text = FormatTime(TimeManager.Instance.GetCurrentTime());
    }

    void OnDestroy()
    {
        TimeManager.OnTimeChanged -= UpdateTimerDisplay;
        TimeManager.OnTimeWarning -= ActivateWarning;
        TimeManager.OnTimeUp -= OnTimeUp;

        ScoreManager.OnScoreChanged -= UpdateScoreDisplay;
    }
}
