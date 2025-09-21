using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int scoreValue = 10;
    public string playerTag = "Player";
    public string hitAnimationTrigger = "Hit"; // Trigger de animación del jugador

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
            Debug.LogWarning($"Enemy {gameObject.name}: No tiene Collider. Añadiendo uno automáticamente.");
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

        // Marcar como recolectado para evitar múltiples activaciones
        isCollected = true;

        Debug.Log($"¡Jugador chocó con enemigo! Puntos: {scoreValue}");

        // Activar animación de choque en el jugador
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
            // Fallback: añadir puntos directamente si no hay EnemyManager
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
            // Buscar en los hijos si no está en el objeto principal
            playerAnimator = playerCollider.GetComponentInChildren<Animator>();
        }

        if (playerAnimator != null && !string.IsNullOrEmpty(hitAnimationTrigger))
        {
            playerAnimator.Play(hitAnimationTrigger);
            Debug.Log($"Activando animación de choque: {hitAnimationTrigger}");
        }
        else
        {
            Debug.LogWarning("No se encontró Animator en el jugador o hitAnimationTrigger está vacío");
        }
    
    }

    // Método para cambiar el valor de puntos dinámicamente
    public void SetScoreValue(int newValue)
    {
        scoreValue = newValue;
    }

    // Método para obtener el valor de puntos
    public int GetScoreValue()
    {
        return scoreValue;
    }

    // Animación opcional de rotación/flotación
    void Update()
    {
        if (isCollected) return;

        // Rotación suave
        transform.Rotate(0f, 50f * Time.deltaTime, 0f);

        // Flotación suave (opcional)
        float floatY = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position = new Vector3(transform.position.x, transform.position.y + floatY * Time.deltaTime, transform.position.z);
    }
}