using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public int totalWaves = 5;
    public float timeBetweenWaves = 5f;
    public int enemiesInWave1 = 5;
    public int addEnemiesPerWave = 1;

    [Header("Boss Settings")]
    public float bossScale = 3f;
    public int bossHealthMultiplier = 10;
    public int bossDamageMultiplier = 3;

    [Header("Spawn Area")]
    public Transform player; // Drag your Player object here in the Inspector
    public float spawnRadius = 20f;

    private int currentWave = 0;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isSpawningWave = false;

    // References to other scripts
    private GameUIManager uiManager;
    private PlayerController playerController;

    void Start()
    {
        // Find the UI Manager
        uiManager = FindFirstObjectByType<GameUIManager>();
        
        // Get the PlayerController component from the player Transform
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player Transform not assigned in EnemyWaveSpawner!");
            return;
        }

        // Start the first wave immediately
        isSpawningWave = true;
        SpawnWave();
        isSpawningWave = false;
    }

    void Update()
    {
        // Don't do anything if we are busy spawning
        if (isSpawningWave) return;

        // Clean the list of any enemies that were destroyed
        spawnedEnemies.RemoveAll(item => item == null);

        // If the list is empty, all enemies are dead
        if (spawnedEnemies.Count == 0)
        {
            // Set flag so this doesn't run multiple times
            isSpawningWave = true;

            if (currentWave < totalWaves)
            {
                // All enemies dead, start the next wave coroutine
                StartCoroutine(NextWaveRoutine());
            }
            else if (currentWave == totalWaves)
            {
                // All regular waves done, spawn the boss
                SpawnBossWave();
                currentWave++; // Increment wave count so this doesn't run again
            }
        }
    }

    IEnumerator NextWaveRoutine()
    {
        Debug.Log("Wave " + currentWave + " cleared! Next wave in " + timeBetweenWaves + "s.");
        // Wait for a few seconds before spawning the next wave
        yield return new WaitForSeconds(timeBetweenWaves);
        SpawnWave();
        isSpawningWave = false; // Allow Update to check for wave clear again
    }

    void SpawnWave()
    {
        currentWave++;
        int enemyCount = enemiesInWave1 + (currentWave - 1) * addEnemiesPerWave;

        Debug.Log("Spawning Wave " + currentWave + " | Enemies: " + enemyCount);
        if (uiManager != null)
        {
            uiManager.UpdateWaveText(currentWave, totalWaves);
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 spawnPos = GetRandomPositionAroundPlayer();
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemy); // Add to our tracking list

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && playerController != null)
            {
                // --- THIS IS THE UPDATED LINE ---
                enemyScript.SetLevel(playerController.currentLevel);
            }
        }
    }

    void SpawnBossWave()
    {
        Debug.Log("Spawning BOSS WAVE!");
        if (uiManager != null)
        {
            uiManager.ShowBossWaveText();
        }

        Vector2 spawnPos = GetRandomPositionAroundPlayer();
        GameObject boss = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        spawnedEnemies.Add(boss); // Add boss to the list

        // Make it a Boss
        boss.transform.localScale = Vector3.one * bossScale; // Make it bigger
        
        Enemy bossScript = boss.GetComponent<Enemy>();
        if (bossScript != null)
        {
            bossScript.isBoss = true; 
            
            int bossLevel = (playerController != null) ? playerController.currentLevel : 1;
            
            // --- THIS IS THE UPDATED LINE ---
            bossScript.SetLevel(bossLevel); 
            
            // Apply multipliers (the SetLevel function handles base stats)
            bossScript.health = bossScript.health * bossHealthMultiplier;
            bossScript.damage = bossScript.damage * bossDamageMultiplier;
        }
    }

    Vector2 GetRandomPositionAroundPlayer()
    {
        // Use normalized to spawn in a ring, not a filled circle
        Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnRadius;
        return (Vector2)player.position + randomOffset;
    }
}