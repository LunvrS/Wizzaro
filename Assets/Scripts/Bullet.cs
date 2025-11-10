using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 10;
    public float lifetime = 3f;
    public float speed = 15f;
    
    private Vector2 direction;
    private Rigidbody2D rb;
    
    // CHANGE THIS: Use Awake() instead of Start()
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // NO GRAVITY - Top-down shooter style
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }

    // This function will now work, because Awake() has already run
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        
        if (rb != null)
        {
            // Set constant velocity - bullets fly straight
            rb.linearVelocity = direction * speed;
        }
        else
        {
            Debug.LogError("Bullet Rigidbody2D is missing!");
        }
        
        // Rotate bullet to face direction of travel
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        
        // Destroy on wall ONLY (not ground, since it's top-down)
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}