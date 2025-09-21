using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccionBotones : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI Nombre;

    void OnEnable()
    {
        ActualizarTextos();
    }
    private void Start()
    {
        if (gameObject.name == "CanvasGameOver")
        {
            AudioVFXManager.Instance.PlaySound(AudioVFXManager.Instance.sonidoPerder);
            AudioVFXManager.Instance.PlayMusic(AudioVFXManager.Instance.musicaGameOver);
        }
        else if (gameObject.name == "CanvasWin")
        {
            AudioVFXManager.Instance.PlayMusic(AudioVFXManager.Instance.musicaWin);
        }
    }
    public void VolverMenu()
    {
        SceneManager.LoadScene("MenuInicial");
    }
    public void Salir()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
    }
    public void ActualizarTextos()
    {
        if (ScoreManager.Instance != null && ScoreText != null)
            ScoreText.text = ScoreManager.Instance.GetCurrentScore().ToString();
        else
            Debug.LogWarning("ScoreText o ScoreManager.Instance está nulo");

        if (AlmacenarNombre.Instance != null && Nombre != null)
            Nombre.text = AlmacenarNombre.Instance.GetPlayerName();
        else
            Debug.LogWarning("Nombre o AlmacenarNombre.Instance está nulo");
    }
}
