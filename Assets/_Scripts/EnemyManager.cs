using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public int totalEnemies = 30;
    public int scorePerEnemy = 10;

    [Header("Spawn Settings")]
    public float minDistanceFromPlayer = 3f;
    public LayerMask groundLayer; // Asigna aquí la capa "Ground"
    public LayerMask obstacleLayerMask; // Para evitar spawns en obstáculos

    [Header("Terrain Reference")]
    public Terrain targetTerrain;

    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("Debug")]
    public bool showSpawnArea = true;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int enemiesCollected = 0;

    public static EnemyManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Buscar jugador automáticamente
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyManager: Enemy Prefab no asignado!");
            return;
        }

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        Debug.Log($"Generando {totalEnemies} enemigos en el terreno...");

        int spawnedCount = 0;
        int maxAttempts = totalEnemies * 10;
        int attempts = 0;

        while (spawnedCount < totalEnemies && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomPosition = GetRandomSpawnPosition();

            if (IsValidSpawnPosition(randomPosition))
            {
                GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript == null)
                    enemyScript = enemy.AddComponent<Enemy>();

                enemyScript.Initialize(scorePerEnemy);

                activeEnemies.Add(enemy);
                spawnedCount++;
            }
        }

        Debug.Log($"Se generaron {spawnedCount}/{totalEnemies} enemigos");
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (targetTerrain == null)
        {
            Debug.LogError("EnemyManager: No se asignó un Terrain en targetTerrain!");
            return Vector3.positiveInfinity;
        }

        Vector3 terrainSize = targetTerrain.terrainData.size;
        Vector3 terrainPos = targetTerrain.transform.position;

        float randomX = Random.Range(terrainPos.x, terrainPos.x + terrainSize.x);
        float randomZ = Random.Range(terrainPos.z, terrainPos.z + terrainSize.z);

        Vector3 spawnPosition = new Vector3(randomX, terrainPos.y + 100f, randomZ);

        // Ajustar altura con raycast al terreno
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, 200f, groundLayer))
        {
            spawnPosition.y = hit.point.y + 0.5f;
        }
        else
        {
            // fallback: altura desde el terreno asignado
            float y = targetTerrain.SampleHeight(spawnPosition) + terrainPos.y;
            spawnPosition.y = y + 0.5f;
        }

        return spawnPosition;
    }


    bool IsValidSpawnPosition(Vector3 position)
    {
        // Evitar que aparezcan cerca del jugador
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(position, playerTransform.position);
            if (distanceToPlayer < minDistanceFromPlayer)
                return false;
        }

        // Verificar obstáculos
        Collider[] overlapping = Physics.OverlapSphere(position, 1f, obstacleLayerMask);
        if (overlapping.Length > 0)
            return false;

        // Evitar que spawneen demasiado juntos
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && Vector3.Distance(position, enemy.transform.position) < 2f)
                return false;
        }

        return true;
    }

    public void OnEnemyCollected(GameObject enemy, int points)
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(points);

        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);

        enemiesCollected++;

        Destroy(enemy);

        if (enemiesCollected >= totalEnemies)
            OnAllEnemiesCollected();
    }

    void OnAllEnemiesCollected()
    {
        Debug.Log("¡Todos los enemigos han sido recolectados!");
        // Aquí puedes añadir lógica extra (bonus, respawn, cambio de nivel, etc.)
    }

    public int GetActiveEnemyCount() => activeEnemies.Count;
    public int GetCollectedCount() => enemiesCollected;
    public int GetTotalEnemies() => totalEnemies;

    public void RespawnEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
            if (enemy != null) Destroy(enemy);

        activeEnemies.Clear();
        enemiesCollected = 0;

        SpawnEnemies();
    }

    void OnDrawGizmosSelected()
    {
        if (!showSpawnArea) return;

        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                terrain.transform.position + terrain.terrainData.size / 2,
                terrain.terrainData.size
            );
        }
    }
}
