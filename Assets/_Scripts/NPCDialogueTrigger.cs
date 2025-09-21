using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject dialoguePanel;   // Panel de UI que contendrá el texto
    public TMP_Text dialogueText;      // Texto que se mostrará (TextMeshPro)
    [TextArea] public string message = "¡Hola, viajero!";

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // Asegurar que el panel esté oculto al inicio
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detecta si el que entra es el jugador
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                if (dialogueText != null)
                {
                    dialogueText.text = message;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Detecta si el que sale es el jugador
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
}

