using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Animation Settings")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;
    public float rotationSpeed = 100f;
    
    [Header("Lifetime Settings")]
    public float lifetime = 10f;
    public float flickerStartTime = 5f;
    public float flickerSpeed = 10f;
    
    private Vector3 startPosition;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private bool isFlickering = false;
    
    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Coin missing SpriteRenderer component!");
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Make the coin bob up and down
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * bobSpeed) * bobHeight, 0);
        
        // Rotate the coin
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        
        // Start flickering after flickerStartTime
        if (timer >= flickerStartTime && !isFlickering)
        {
            isFlickering = true;
        }
        
        // Handle flickering
        if (isFlickering && spriteRenderer != null)
        {
            float alpha = Mathf.PingPong(Time.time * flickerSpeed, 1f);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        
        // Destroy after lifetime
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Add coin to player's collection
                player.CollectCoin();
                
                // Destroy the coin
                Destroy(gameObject);
            }
        }
    }
}