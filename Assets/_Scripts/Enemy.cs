using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int scoreValue = 10;
    public string playerTag = "Player";
    public string hitAnimationTrigger = "Hit"; // Trigger de animaci�n del jugador

    // Variables privadas
    private bool isCollected = false;

    void Start()
    {

        // Asegurarse de que el objeto tenga un Collider configurado como Trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"Enemy {gameObject.name}: No tiene Collider. A�adiendo uno autom�ticamente.");
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = 1f;
        }
    }

    public void Initialize(int points)
    {
        scoreValue = points;
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si es el jugador y no ha sido recolectado
        if (isCollected || !other.CompareTag(playerTag)) return;

        // Marcar como recolectado para evitar m�ltiples activaciones
        isCollected = true;

        Debug.Log($"�Jugador choc� con enemigo! Puntos: {scoreValue}");

        // Activar animaci�n de choque en el jugador
        ActivatePlayerHitAnimation(other);
        AudioVFXManager.Instance.PlaySound(AudioVFXManager.Instance.sonidoChoque);
        AudioVFXManager.Instance.PlayVFX(AudioVFXManager.Instance.VFXChoque, this.transform);

        // Notificar al EnemyManager
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyCollected(gameObject, scoreValue);
        }
        else
        {
            // Fallback: a�adir puntos directamente si no hay EnemyManager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
            }

            // Destruir el objeto
            Destroy(gameObject);
        }
    }

    void ActivatePlayerHitAnimation(Collider playerCollider)
    {
        // Buscar el Animator en el jugador
        Animator playerAnimator = playerCollider.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            // Buscar en los hijos si no est� en el objeto principal
            playerAnimator = playerCollider.GetComponentInChildren<Animator>();
        }

        if (playerAnimator != null && !string.IsNullOrEmpty(hitAnimationTrigger))
        {
            playerAnimator.Play(hitAnimationTrigger);
            Debug.Log($"Activando animaci�n de choque: {hitAnimationTrigger}");
        }
        else
        {
            Debug.LogWarning("No se encontr� Animator en el jugador o hitAnimationTrigger est� vac�o");
        }
    
    }

    // M�todo para cambiar el valor de puntos din�micamente
    public void SetScoreValue(int newValue)
    {
        scoreValue = newValue;
    }

    // M�todo para obtener el valor de puntos
    public int GetScoreValue()
    {
        return scoreValue;
    }

    // Animaci�n opcional de rotaci�n/flotaci�n
    void Update()
    {
        if (isCollected) return;

        // Rotaci�n suave
        transform.Rotate(0f, 50f * Time.deltaTime, 0f);

        // Flotaci�n suave (opcional)
        float floatY = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position = new Vector3(transform.position.x, transform.position.y + floatY * Time.deltaTime, transform.position.z);
    }
}