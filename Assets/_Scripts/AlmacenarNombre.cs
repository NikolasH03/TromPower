using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class AlmacenarNombre : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField nameInputField;
    public Button nextButton;

    [Header("Settings")]
    public string nextSceneName = "Gameplay"; // Nombre de la siguiente escena

    // Singleton instance
    public static AlmacenarNombre Instance { get; private set; }

    // Variable que almacena el nombre del jugador
    [HideInInspector]
    public string playerName = "";

    void Awake()
    {
        // Implementar Singleton pattern con DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Configurar el botón para que llame a la función cuando se presione
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonPressed);
        }

        // Opcional: permitir presionar Enter para continuar
        if (nameInputField != null)
        {
            nameInputField.onEndEdit.AddListener(OnInputFieldEndEdit);
        }

        AudioVFXManager.Instance.PlayMusic(AudioVFXManager.Instance.musicaMenu);
    }
    void OnInputFieldEndEdit(string input)
    {
        // Si el jugador presiona Enter, también proceder
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnNextButtonPressed();
        }
    }

    public void OnNextButtonPressed()
    {
        // Obtener el texto del InputField
        if (nameInputField != null)
        {
            string inputText = nameInputField.text.Trim();

            // Validar que el nombre no esté vacío
            if (string.IsNullOrEmpty(inputText))
            {
                Debug.LogWarning("El nombre no puede estar vacío!");
                // Opcional: mostrar mensaje de error al jugador
                return;
            }

            // Almacenar el nombre del jugador
            playerName = inputText;

            Debug.Log($"Nombre del jugador guardado: {playerName}");

            // Cargar la siguiente escena
            LoadNextScene();
        }
        else
        {
            Debug.LogError("InputField no asignado en el inspector!");
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Nombre de la siguiente escena no configurado!");
        }
    }

    // Método público para obtener el nombre del jugador desde otras escenas
    public string GetPlayerName()
    {
        return playerName;
    }

    // Método público para verificar si hay un nombre guardado
    public bool HasPlayerName()
    {
        return !string.IsNullOrEmpty(playerName);
    }

    // Método para limpiar el nombre (útil para reiniciar)
    public void ClearPlayerName()
    {
        playerName = "";
    }

    void OnDestroy()
    {
        // Limpiar listeners cuando se destruye el objeto
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonPressed);
        }

        if (nameInputField != null)
        {
            nameInputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
        }
    }
}
