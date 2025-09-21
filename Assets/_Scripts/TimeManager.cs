using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    public float totalTime = 120f; // 2 minutos en segundos
    public string gameOverSceneName = "GameOver";

    [Header("Events")]
    public bool enableWarningTime = true;
    public float warningTime = 30f; // Tiempo restante para activar warning

    // Variables privadas
    private float currentTime;
    private bool isTimeRunning = true;
    private bool hasWarningTriggered = false;
    private bool isGameOver = false;
    private bool isPaused = false; // NUEVO

    // Eventos para notificar cambios de tiempo
    public static event Action<float> OnTimeChanged;
    public static event Action OnTimeWarning;
    public static event Action OnTimeUp;

    private bool EscenaCargada = false;
    // Singleton para acceso fácil
    public static TimeManager Instance { get; private set; }

    void Awake()
    {
        // Implementar singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializar el tiempo
        currentTime = totalTime;

        // Notificar el tiempo inicial
        OnTimeChanged?.Invoke(currentTime);

        Debug.Log($"TimeManager iniciado. Tiempo total: {totalTime} segundos");

        AudioVFXManager.Instance.PlayMusic(AudioVFXManager.Instance.musicaJuego);
    }

    void Update()
    {
        if (!isTimeRunning || isGameOver || isPaused) return;

        // Reducir el tiempo
        currentTime -= Time.deltaTime;

        // Notificar cambio de tiempo
        OnTimeChanged?.Invoke(currentTime);

        // Verificar warning
        CheckWarning();

        // Verificar si el tiempo se acabó
        if (currentTime <= 0f)
        {
            TimeUp();
        }

        if (!EscenaCargada)
        {
            LoadWinScene();
        }
        
    }

    void CheckWarning()
    {
        if (enableWarningTime && !hasWarningTriggered && currentTime <= warningTime)
        {
            hasWarningTriggered = true;
            OnTimeWarning?.Invoke();
            Debug.Log("¡Advertencia! Tiempo restante crítico");
            AudioVFXManager.Instance.PlayMusic(AudioVFXManager.Instance.musicaUltimosMinutos);
        }
    }

    void TimeUp()
    {
        currentTime = 0f;
        isTimeRunning = false;
        isGameOver = true;

        // Notificar que el tiempo se acabó
        OnTimeUp?.Invoke();

        Debug.Log("¡Tiempo agotado! Cargando escena de Game Over");

        // Cargar escena de game over después de un pequeño delay
        Invoke(nameof(LoadGameOverScene), 1f);
    }

    void LoadGameOverScene()
    {
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else
        {
            Debug.LogError("TimeManager: Nombre de escena de Game Over no configurado");
        }
    }
    void LoadWinScene()
    {
        if (ScoreManager.Instance.GetCurrentScore()>=300)
        {
            EscenaCargada = true;
            isTimeRunning = false;
            SceneManager.LoadScene("WinScreen");
        }
    }

    // Métodos públicos
    public float GetCurrentTime()
    {
        return currentTime;
    }

    public float GetTotalTime()
    {
        return totalTime;
    }

    public float GetTimePercentage()
    {
        return currentTime / totalTime;
    }

    public bool IsTimeRunning()
    {
        return isTimeRunning;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    // Métodos para controlar el tiempo y pausa
    public void PauseTime()
    {
        if (!isGameOver)
        {
            isPaused = true;
            isTimeRunning = false;
            Time.timeScale = 0f; // Pausar todo
            Debug.Log("Juego pausado");
        }
    }

    public void ResumeTime()
    {
        if (!isGameOver)
        {
            isPaused = false;
            isTimeRunning = true;
            Time.timeScale = 1f; // Reanudar todo
            Debug.Log("Juego reanudado");
        }
    }

    public void AddTime(float timeToAdd)
    {
        if (!isGameOver)
        {
            currentTime += timeToAdd;
            currentTime = Mathf.Clamp(currentTime, 0f, totalTime);
            OnTimeChanged?.Invoke(currentTime);
            Debug.Log($"Tiempo añadido: {timeToAdd}s. Tiempo actual: {currentTime}s");
        }
    }

    public void SubtractTime(float timeToSubtract)
    {
        if (!isGameOver)
        {
            currentTime -= timeToSubtract;
            if (currentTime <= 0f)
            {
                TimeUp();
            }
            else
            {
                OnTimeChanged?.Invoke(currentTime);
                Debug.Log($"Tiempo reducido: {timeToSubtract}s. Tiempo actual: {currentTime}s");
            }
        }
    }

    // Formatear tiempo para display (MM:SS)
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnDestroy()
    {
        // Limpiar eventos al destruir
        OnTimeChanged = null;
        OnTimeWarning = null;
        OnTimeUp = null;
    }
}
