using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    public float rotationSpeed = 100f;
    
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        // Make the coin bob up and down
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * bobSpeed) * bobHeight, 0);
        
        // Rotate the coin
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Find player controller and add coin
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Play sound effect if you have one
                
                // Add coin to player's collection
                player.CollectCoin();
                
                // Destroy the coin
                Destroy(gameObject);
            }
        }
    }
}