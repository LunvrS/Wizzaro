using UnityEngine;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject enemyPrefab;
    public int totalWaves = 5;               // จำนวน Wave
    public float timeBetweenWaves = 5f;      // เวลาระหว่าง Wave
    public int enemiesInWave1 = 5;           // Wave แรก 2 ตัว
    public int addEnemiesPerWave = 1;        // Wave ต่อไปเพิ่มทีละ 1 ตัว

    [Header("Spawn Area")]
    public Transform player;                 // เพื่อให้ spawn รอบผู้เล่น
    public float spawnRadius = 20f;          // ระยะรัศมีรอบผู้เล่น

    private float timer = 0f;
    private int currentWave = 0;

    // เก็บรายชื่อ Enemy ที่เกิด
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        if (currentWave >= totalWaves)
        {
            // ตรวจสอบว่า Enemy หมดหรือยัง ถ้าหมด Destroy WaveSpawner
            spawnedEnemies.RemoveAll(item => item == null); // ลบ Enemy ที่ถูก Destroy ไปแล้ว
            if (spawnedEnemies.Count == 0)
            {
                Destroy(gameObject);
            }
            return;
        }

        timer += Time.deltaTime;

        if (timer >= timeBetweenWaves)
        {
            SpawnWave();
            timer = 0f;  // รีเซ็ตเวลาสำหรับ Wave ต่อไป
        }
    }

    void SpawnWave()
    {
        currentWave++;

        int enemyCount = enemiesInWave1 + (currentWave - 1) * addEnemiesPerWave;

        Debug.Log("Spawning Wave " + currentWave + " | Enemy: " + enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 spawnPos = GetRandomPositionAroundPlayer();
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(enemy); // เก็บไว้เพื่อตรวจสอบตอน Wave หมด
            Debug.Log("Spawn Enemy at: " + spawnPos);
        }
    }

    Vector2 GetRandomPositionAroundPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform not assigned in EnemyWaveSpawner!");
            return Vector2.zero;
        }

        // สุ่มตำแหน่งรอบผู้เล่น
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        return (Vector2)player.position + randomOffset;
    }
}
