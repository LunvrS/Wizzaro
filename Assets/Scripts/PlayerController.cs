using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Movement Variables
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float moveInput;
    private bool facingRight = true;

    // Shotgun Variables
    [Header("Shotgun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float recoilForce = 10f;
    public float airRecoilMultiplier = 1.5f;
    public float spreadAngle = 15f;
    public int pelletCount = 3;
    public float shootCooldown = 0.5f;
    private bool canShoot = true;
    public GameObject crosshair;
    
    // Shotgun Sprite
    [Header("Shotgun Sprite")]
    public GameObject shotgunSprite; // Reference to your shotgun sprite GameObject
    public Vector2 shotgunOffset = new Vector2(0.5f, 0f); // Offset from player center
    public float shotgunDistance = 0.75f; // Distance from player center

    // Health Variables
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // UI Elements
    [Header("UI Elements")]
    public TMPro.TextMeshProUGUI coinText; // Changed to TextMeshProUGUI
    public TMPro.TextMeshProUGUI timerText; // Changed to TextMeshProUGUI
    private float gameTimer = 0f;
    public int coinsCollected = 0;
    public int totalCoinsNeeded = 20;
    
    // Animation
    private Animator animator;
    
    // Game state
    public GameObject gameOverPanel;
    public GameObject winPanel;
    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        
        // Check for UI components
        if (coinText == null)
            Debug.LogWarning("Coin Text UI component is not assigned in the inspector!");
        if (timerText == null)
            Debug.LogWarning("Timer Text UI component is not assigned in the inspector!");
            
        UpdateHealthUI();
        UpdateCoinUI();
        
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        
        // If shotgun sprite wasn't assigned in inspector, create one
        if (shotgunSprite == null)
        {
            Debug.LogWarning("No shotgun sprite assigned. Please assign a shotgun sprite in the inspector.");
        }
    }

    void Update()
    {
        if (isGameOver) return;
        
        // Update timer
        gameTimer += Time.deltaTime;
        UpdateTimerUI();
        
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Debug ground check
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed. isGrounded: " + isGrounded + 
                      ", Position: " + groundCheck.position + 
                      ", Radius: " + groundCheckRadius +
                      ", Layer: " + groundLayer.value);
        }
        
        // Handle movement
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        // Animation states
        UpdateAnimationState();
        
        // Flip character when changing direction
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }
        
        // Handle jumping
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            Jump();
        }
        
        // Handle shooting
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Shoot();
        }
        
        // Update crosshair position
        UpdateCrosshairPosition();
        
        // Update shotgun position and rotation
        UpdateShotgunPosition();
    }

    void UpdateAnimationState()
    {
        if (animator != null)
        {
            // Check if we have animation parameters before setting them
            // You can uncomment these lines once you've set up your animator with these parameters
            
             animator.SetFloat("speed", Mathf.Abs(moveInput));
             animator.SetBool("IsGrounded", isGrounded);
             animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        // Change the way we flip to avoid rapid flipping issues
        facingRight = !facingRight;
        
        // Instead of rotating, we'll flip the scale
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger("jump");
        
        // Debug log to confirm jump function is called
        Debug.Log("Jump triggered with force: " + jumpForce);
        
        // Comment out the animation trigger until you have set up your animator
        // if (animator != null)
        // {
        //     animator.SetTrigger("Jump");
        // }
    }

    void UpdateShotgunPosition()
    {
        if (shotgunSprite != null)
        {
            // Get mouse position in world coordinates
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Calculate direction to mouse
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            
            // Position shotgun at an offset from player in the direction of the mouse
            Vector2 shotgunPosition = (Vector2)transform.position + direction * shotgunDistance;
            
            // Apply additional offset based on character facing
            float horizontalOffset = shotgunOffset.x * (facingRight ? 1 : -1);
            shotgunPosition += new Vector2(horizontalOffset, shotgunOffset.y);
            
            // Update shotgun position
            shotgunSprite.transform.position = shotgunPosition;
            
            // Calculate angle for shotgun rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Update shotgun rotation
            shotgunSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Flip shotgun sprite vertically if pointing left (optional, depends on your sprite)
            if (direction.x < 0 && shotgunSprite.transform.localScale.y > 0)
            {
                shotgunSprite.transform.localScale = new Vector3(
                    shotgunSprite.transform.localScale.x,
                    -shotgunSprite.transform.localScale.y,
                    shotgunSprite.transform.localScale.z
                );
            }
            else if (direction.x >= 0 && shotgunSprite.transform.localScale.y < 0)
            {
                shotgunSprite.transform.localScale = new Vector3(
                    shotgunSprite.transform.localScale.x,
                    Mathf.Abs(shotgunSprite.transform.localScale.y),
                    shotgunSprite.transform.localScale.z
                );
            }
        }
    }

    void Shoot()
    {
        // Apply cooldown
        canShoot = false;
        StartCoroutine(ShootCooldown());
        
        // Direction to mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;
        
        // Play shotgun animation if needed
        if (shotgunSprite != null)
        {
            // You can add animation or visual feedback here
            StartCoroutine(ShotgunRecoilAnimation());
        }
        
        // Spawn pellets with spread
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate spread angle
            float spreadRadians = Random.Range(-spreadAngle, spreadAngle) * Mathf.Deg2Rad;
            Vector2 spreadDirection = RotateVector(direction, spreadRadians);
            
            // Create bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            // Apply force to bullet
            if (bulletRb != null)
            {
                bulletRb.AddForce(spreadDirection * bulletForce, ForceMode2D.Impulse);
            }
            
            // Destroy bullet after time
            Destroy(bullet, 2f);
        }
        
        // Apply recoil force in opposite direction
        Vector2 recoilDirection = -direction;
        float recoilMultiplier = isGrounded ? 1f : airRecoilMultiplier;
        rb.AddForce(recoilDirection * recoilForce * recoilMultiplier, ForceMode2D.Impulse);
    }
    
    IEnumerator ShotgunRecoilAnimation()
    {
        // Simple recoil animation
        if (shotgunSprite != null)
        {
            Vector3 originalPosition = shotgunSprite.transform.localPosition;
            
            // Move back slightly
            shotgunSprite.transform.localPosition -= shotgunSprite.transform.right * 0.2f;
            
            yield return new WaitForSeconds(0.05f);
            
            // Return to original position
            shotgunSprite.transform.localPosition = originalPosition;
        }
    }
    
    Vector2 RotateVector(Vector2 vector, float angle)
    {
        return new Vector2(
            vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle)
        );
    }
    
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
    
    void UpdateCrosshairPosition()
    {
        if (crosshair != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            crosshair.transform.position = mousePos;
        }
    }
    
    public void CollectCoin()
    {
        coinsCollected++;
        UpdateCoinUI();
        
        // Check if all coins collected for win condition
        if (coinsCollected >= totalCoinsNeeded)
        {
            // You could trigger something here or wait for the flag
        }
    }
    
    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coinsCollected + "/" + totalCoinsNeeded + " Coins";
        }
        else
        {
            Debug.LogWarning("Coin Text is not assigned in the inspector!");
        }
    }
    
    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60);
            int seconds = Mathf.FloorToInt(gameTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.LogWarning("Timer Text is not assigned in the inspector!");
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    void UpdateHealthUI()
    {
        if (heartImages.Length > 0)
        {
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (i < currentHealth)
                {
                    heartImages[i].sprite = fullHeart;
                }
                else
                {
                    heartImages[i].sprite = emptyHeart;
                }
            }
        }
    }
    
    public void WinGame()
    {
        isGameOver = true;
        if (winPanel) winPanel.SetActive(true);
        rb.linearVelocity = Vector2.zero;
    }
    
    void GameOver()
    {
        isGameOver = true;
        if (gameOverPanel) gameOverPanel.SetActive(true);
        rb.linearVelocity = Vector2.zero;
    }
    
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle falling out of map
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            GameOver();
        }
    }
    
    private void OnDrawGizmos()
    {
        // Draw ground check radius
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}