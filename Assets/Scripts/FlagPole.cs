using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    [Header("Visual Elements (Optional)")]
    public GameObject flagUpSprite;
    public GameObject flagDownSprite;
    
    private bool hasWon = false;
    
    private void Start()
    {
        // Initialize flag visuals if assigned
        if (flagUpSprite != null) flagUpSprite.SetActive(false);
        if (flagDownSprite != null) flagDownSprite.SetActive(true);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasWon) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                hasWon = true;
                
                // Change flag visuals
                if (flagUpSprite != null) flagUpSprite.SetActive(true);
                if (flagDownSprite != null) flagDownSprite.SetActive(false);
                
                // Trigger win
                player.WinGame();
            }
        }
    }
}