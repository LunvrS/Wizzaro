using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public int health = 2;
    public float moveSpeed = 2f;
    public int damageAmount = 1;
    public float detectionRange = 7f;
    public GameObject coinPrefab;
    
    private Transform player;
    private Rigidbody2D rb;
    private bool isDead = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Find player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Enemy can't find player! Make sure your player has the 'Player' tag.");
        }
    }
    
    void Update()
    {
        if (isDead) return;
        
        // Re-try to find player if null
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }
        
        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Move towards player if in range
        if (distanceToPlayer <= detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // Stop moving
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    
    void MoveTowardsPlayer()
    {
        // Calculate direction to player
        Vector2 direction = (player.position - transform.position).normalized;
        
        // Move towards player on X axis only
        float moveDirection = direction.x > 0 ? 1 : -1;
        
        // Apply movement using velocity
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on movement direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (moveDirection < 0);
        }
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy took " + damage + " damage. Health: " + health);
        
        // Flash red when hit
        StartCoroutine(FlashRed());
        
        if (health <= 0 && !isDead)
        {
            Die();
        }
    }
    
    IEnumerator FlashRed()
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
        Debug.Log("Enemy died");
        
        // Disable collision
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Disable rigidbody physics
        if (rb != null) rb.simulated = false;
        
        // Spawn coin with 50% chance
        if (Random.value > 0.5f && coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
        
        // Make it look dead
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray and transparent
            transform.Rotate(0, 0, 90); // Rotate on its side
        }
        
        // Destroy after delay
        Destroy(gameObject, 1.0f);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemy collided with player");
            
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && !isDead)
            {
                player.TakeDamage(damageAmount);
                
                // Apply knockback to player
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRb.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);
                }
            }
        }
    }
    
    // This method gets called when a bullet hits the enemy
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit by bullet
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }
    
    private void OnDrawGizmos()
    {
        // Visualize detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}