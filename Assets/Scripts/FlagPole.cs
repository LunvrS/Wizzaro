using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPole : MonoBehaviour
{
    public int requiredCoins = 20;
    public GameObject flagUpSprite;
    public GameObject flagDownSprite;
    
    private void Start()
    {
        // Make sure the flag is down at start
        if (flagUpSprite != null) flagUpSprite.SetActive(false);
        if (flagDownSprite != null) flagDownSprite.SetActive(true);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            // Check if player has enough coins
            if (player != null)
            {
                // Check if we should raise the flag (player has all coins)
                if (player.coinsCollected >= requiredCoins)
                {
                    // Raise the flag
                    if (flagUpSprite != null) flagUpSprite.SetActive(true);
                    if (flagDownSprite != null) flagDownSprite.SetActive(false);
                    
                    // Trigger win condition
                    player.WinGame();
                }
                else
                {
                    // Player doesn't have enough coins yet - could show message here
                    // For example, you could show a UI message that says "Collect more coins!"
                }
            }
        }
    }
}