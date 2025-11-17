using UnityEngine;
using System.Collections;
using TMPro; 

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int health = 50;
    public float moveSpeed = 2f;
    public int damage = 20;
    public float damageInterval = 1f;
    
    [Header("Drop Settings")]
    public GameObject coinPrefab;
    [Range(0f, 1f)]
    public float coinDropChance = 0.5f;
    
    [Header("Level Display")]
    public GameObject levelTextPrefab; 
    public Vector3 levelTextOffset = new Vector3(0, 1f, 0);
    
    public bool isBoss = false;
    public int enemyLevel = 1;
    
    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private float lastDamageTime = 0f;
    private GameObject levelTextInstance;
    private TMPro.TextMeshPro levelText; 
    private GameUIManager uiManager; 

    // --- CHANGE 1: Add variables to store the flash coroutine and original color ---
    private Coroutine flashRoutine;
    private Color originalColor;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        uiManager = FindFirstObjectByType<GameUIManager>(); 
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        
        FindPlayer();
        CreateLevelText();
        
        if (isBoss)
        {
            ApplyBossEffects();
        }

        originalColor = spriteRenderer.color;
    }
    
    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Enemy can't find player! Make sure your player has the 'Player' tag.");
        }
    }
    
    void Update()
    {
        if (isDead) return;
        
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        MoveTowardsPlayer();
        
        if (levelTextInstance != null)
        {
            levelTextInstance.transform.position = transform.position + levelTextOffset;
        }
    }
    
    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction.x < 0);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        health -= damageAmount;
        
        // --- CHANGE 3: Stop any old flash and start a new one ---
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashEffect());
        // --- END CHANGE ---
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    IEnumerator FlashEffect()
    {
        // Set to flash color
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Return to the saved original color
        spriteRenderer.color = originalColor;
        
        // Clear the coroutine reference
        flashRoutine = null;
    }
    
    void Die()
    {
        isDead = true;

        // Stop the flash coroutine if the enemy dies while flashing
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        
        // Check for Boss Death
        if (isBoss && uiManager != null)
        {
            uiManager.ShowWinScreen();
            
            EnemyWaveSpawner spawner = FindFirstObjectByType<EnemyWaveSpawner>();
            if (spawner != null)
            {
                Destroy(spawner.gameObject);
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        if (levelTextInstance != null)
        {
            Destroy(levelTextInstance);
        }
        
        if (Random.value <= coinDropChance && coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        
        // Set death color (make sure it's not red)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }
        
        Destroy(gameObject, 0.5f);
    }
    
    public void SetLevel(int level)
    {
        enemyLevel = level;
        
        if (!isBoss)
        {
            health = 50 + (level - 1) * 20; 
            damage = 20 + (level - 1) * 5;  
            moveSpeed = 2f + (level - 1) * 0.2f; 
        }
        
        if (levelText != null)
        {
            UpdateLevelText();
        }
    }
    
    void CreateLevelText()
    {
        if (levelTextPrefab != null)
        {
            levelTextInstance = Instantiate(levelTextPrefab, transform.position + levelTextOffset, Quaternion.identity);
            levelText = levelTextInstance.GetComponent<TMPro.TextMeshPro>();
        }
        else
        {
            GameObject textObj = new GameObject("EnemyLevelText");
            textObj.transform.position = transform.position + levelTextOffset;
            textObj.transform.SetParent(null); 
            
            levelText = textObj.AddComponent<TMPro.TextMeshPro>();
            levelText.alignment = TMPro.TextAlignmentOptions.Center;
            levelText.fontSize = 6;
            levelText.sortingOrder = 10; 
            
            levelTextInstance = textObj;
        }
        
        UpdateLevelText();
    }
    
    void UpdateLevelText()
    {
        if (levelText != null)
        {
            if (isBoss)
            {
                levelText.text = $"<color=red>BOSS LV.{enemyLevel:00}</color>";
                levelText.fontSize = 7;
            }
            else
            {
                levelText.text = $"LV.{enemyLevel:00}";
                levelText.color = Color.yellow;
            }
        }
    }
    
    void ApplyBossEffects()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f); // Reddish tint
        }
        
        if (levelText != null)
        {
            UpdateLevelText();
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;
        
        if (collision.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                    lastDamageTime = Time.time;
                }
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        
        if (collision.CompareTag("Bullet"))
        {
            Bullet bullet = collision.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
            }
            Destroy(collision.gameObject);
        }
    }
}