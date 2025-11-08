using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 1;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // Apply damage to player
                player.TakeDamage(damage);
                
                // Apply knockback
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    // Add a bit of upward force
                    knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 0.5f;
                    playerRb.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse);
                }
            }
        }
    }
}