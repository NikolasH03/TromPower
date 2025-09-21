using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    public int startingScore = 0;
    public int maxScore = 999999;

    // Variables privadas
    private int currentScore;

    // Eventos para notificar cambios de score
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnScoreAdded;

    // Singleton para acceso fácil
    public static ScoreManager Instance { get; private set; }

    void Awake()
    {
        // Implementar singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener entre escenas para high scores
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializar score
        currentScore = startingScore;

        // Notificar score inicial
        OnScoreChanged?.Invoke(currentScore);

        Debug.Log($"ScoreManager iniciado. Score inicial: {currentScore}");
    }

    public void AddScore(int points)
    {
        if (points <= 0) return;

        int oldScore = currentScore;
        currentScore = Mathf.Clamp(currentScore + points, 0, maxScore);

        // Notificar eventos
        OnScoreAdded?.Invoke(points);
        OnScoreChanged?.Invoke(currentScore);

        Debug.Log($"Score añadido: +{points}. Score total: {currentScore}");
    }

    public void SubtractScore(int points)
    {
        if (points <= 0) return;

        int oldScore = currentScore;
        currentScore = Mathf.Max(currentScore - points, 0);

        // Notificar cambio
        OnScoreChanged?.Invoke(currentScore);

        Debug.Log($"Score reducido: -{points}. Score total: {currentScore}");
    }

    // Métodos públicos para obtener información
    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void SetScore(int newScore)
    {
        int oldScore = currentScore;
        currentScore = Mathf.Clamp(newScore, 0, maxScore);

        OnScoreChanged?.Invoke(currentScore);

        Debug.Log($"Score establecido a: {currentScore}");
    }

    public void ResetScore()
    {
        currentScore = startingScore;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log("Score reiniciado");
    }
    void OnDestroy()
    {
        // Limpiar eventos
        OnScoreChanged = null;
        OnScoreAdded = null;
    }
}