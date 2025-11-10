using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int health = 50;
    public float moveSpeed = 2f;
    public int damage = 20;
    public float damageInterval = 1f; // Damage player every X seconds
    
    [Header("Drop Settings")]
    public GameObject coinPrefab;
    [Range(0f, 1f)]
    public float coinDropChance = 0.5f;
    
    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    private float lastDamageTime = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Disable gravity for top-down movement
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        
        // Find player
        FindPlayer();
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
        
        // Re-find player if null
        if (player == null)
        {
            FindPlayer();
            return;
        }
        
        // Move towards player
        MoveTowardsPlayer();
    }
    
    void MoveTowardsPlayer()
    {
        // Calculate direction to player (2D plane - both X and Y)
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Move towards player on both axes
        rb.linearVelocity = direction * moveSpeed;
        
        // Flip sprite based on movement direction (optional)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction.x < 0);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        
        health -= damageAmount;
        
        // Flash effect when hit
        StartCoroutine(FlashEffect());
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
    
    void Die()
    {
        isDead = true;
        
        // Disable components
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        // Spawn coin based on chance
        if (Random.value <= coinDropChance && coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        
        // Visual death effect
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }
        
        // Destroy after delay
        Destroy(gameObject, 0.5f);
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;
        
        // Damage player on contact (with interval)
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
        
        // Handle bullet collision
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