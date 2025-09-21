using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private float _speed;
    public float rotationSpeed = 720f; // Grados por segundo

    [Header("Animation")]
    public Animator animator;
    public float x, y;
    public string hitAnimation = "Hit"; // Esto sí puede ser trigger aún

    [Header("Components")]
    public Transform playerTransform;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;

    // Variables privadas
    private Vector3 movement;
    private bool isMoving = false;
    private bool isPaused = false;

    // Referencias a componentes
    private Rigidbody rb;

    void Start()
    {
        // Obtener referencias a componentes
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("InputManager: No se encontró Rigidbody en el jugador!");
        }

        if (playerTransform == null)
        {
            playerTransform = transform;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("InputManager: No se encontró Animator. Las animaciones no funcionarán.");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Capturar input del jugador
        GetPlayerInput();

        // Actualizar parámetros de animación
        HandleAnimations();

        // Rotar el jugador hacia la dirección de movimiento
        if (isMoving)
        {
            RotatePlayer();
        }
    }

    void FixedUpdate()
    {
        // Mover el jugador usando física
        ValoresAnimacionMovimiento();

        mover();
    }

    void GetPlayerInput()
    {


        // Dirección de la cámara (solo plano XZ)
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        //// Construir dirección de movimiento relativa a la cámara
        //movement = (camForward * vertical + camRight * horizontal).normalized;

        //isMoving = movement.magnitude > 0.1f;

        //// Aquí enviamos directamente al animator los valores de input
        //if (animator != null)
        //{
        //    animator.SetFloat(paramX, horizontal, 0.1f, Time.deltaTime); // con damping
        //    animator.SetFloat(paramY, vertical, 0.1f, Time.deltaTime);
        //}
    }

    //void MovePlayer()
    //{
    //    if (rb != null && isMoving)
    //    {
    //        Vector3 moveDirection = movement * moveSpeed;
    //        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    //    }
    //    else if (rb != null)
    //    {
    //        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    //    }
    //}

    void RotatePlayer()
    {
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            playerTransform.rotation = Quaternion.RotateTowards(
                playerTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
    public void ValoresAnimacionMovimiento()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");


        animator.SetFloat("FloatX", x);
        animator.SetFloat("FloatY", y);

    }
    void HandleAnimations()
    {
        if (animator == null) return;

        // Ejemplo: trigger manual de golpe
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(hitAnimation);
        }
    }

    // Método público para obtener si el jugador se está moviendo
    public bool IsMoving() => isMoving;

    public Vector3 GetMovementDirection() => movement;

    public float GetCurrentSpeed()
    {
        if (rb != null)
            return new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        return 0f;
    }

    //public void SetInputEnabled(bool enabled)
    //{
    //    this.enabled = enabled;
    //    if (!enabled && rb != null)
    //    {
    //        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    //        if (animator != null)
    //        {
    //            animator.SetFloat(paramX, 0f);
    //            animator.SetFloat(paramY, 0f);
    //        }
    //    }
    //}
    public void mover()
    {
        // Leer input del Old Input System
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 inputVector = new Vector2(horizontal, vertical);
        Vector3 inputDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

        // Velocidad objetivo
        float targetSpeed = (inputVector == Vector2.zero) ? 0.0f : moveSpeed;

        // Velocidad actual del rigidbody (solo horizontal)
        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;

        float speedOffset = 0.5f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * 5f);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // Solo mover si hay input
        if (inputVector != Vector2.zero)
        {
            // Calcular rotación hacia donde se mueve el jugador en relación a la cámara
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                   Camera.main.transform.eulerAngles.y;

            Quaternion rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rotation,
                rotationSpeed * Time.deltaTime
            );

            // Dirección hacia adelante relativa a la cámara
            Vector3 targetDirection = rotation * Vector3.forward;

            rb.MovePosition(transform.position +
                (targetDirection.normalized * _speed * Time.deltaTime));
        }
        else
        {
            // Si no hay input, mantener posición en XZ (solo gravedad del rigidbody)
            rb.MovePosition(transform.position +
                new Vector3(0.0f, rb.velocity.y, 0.0f) * Time.deltaTime);
        }
    }



    void PauseGame()
    {
        TimeManager.Instance.PauseTime();
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        isPaused = true;
        Debug.Log("Juego en pausa");
    }

    public void ResumeGame()
    {
        TimeManager.Instance.ResumeTime();
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        isPaused = false;
        Debug.Log("Juego reanudado");
    }

}
