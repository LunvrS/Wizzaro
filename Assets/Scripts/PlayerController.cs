using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float knockbackResistance = 0.5f; // How much knockback affects player
    
    [Header("Map Bounds")]
    public bool useMapBounds = true;
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -50f;
    public float maxY = 50f;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public float shootCooldown = 0.5f;
    public float spreadAngle = 10f;
    public int pelletCount = 1;
    public int baseDamage = 10; // Base damage for bullets
    
    [Header("Level System")]
    public int currentLevel = 1;
    public int coinsCollected = 0;
    public int coinsToLevelUp = 5;
    public float damageMultiplier = 1f; // Increases with levels
    
    [Header("Visuals")]
    public GameObject shotgunSprite;
    public float shotgunDistance = 0.75f;
    
    // Private variables
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 mousePos;
    private bool canShoot = true;
    private bool isGameOver = false;
    private SpriteRenderer spriteRenderer;
    private GameUIManager uiManager;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Disable gravity for top-down movement
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        // Initialize health
        currentHealth = maxHealth;
        
        // Find UI Manager
        uiManager = FindFirstObjectByType<GameUIManager>();
        
        // Update UI
        UpdateUI();
        
        // Hide cursor and show crosshair if needed
        Cursor.visible = true;
    }
    
    void Update()
    {
        if (isGameOver) return;
        
        // Get input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // Get mouse position
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Handle shooting
        if (Input.GetButton("Fire1") && canShoot)
        {
            Shoot();
        }
        
        // Update shotgun position
        UpdateShotgunPosition();
        
        // Flip sprite based on mouse direction
        FlipSprite();
    }
    
    void FixedUpdate()
    {
        if (isGameOver) return;
        
        // Move player
        Vector2 targetVelocity = movement.normalized * moveSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 0.5f);
        
        // Enforce map bounds
        if (useMapBounds)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            transform.position = clampedPosition;
        }
    }
    
    void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            // Flip based on mouse position relative to player
            spriteRenderer.flipX = (mousePos.x < transform.position.x);
        }
    }
    
    void UpdateShotgunPosition()
    {
        if (shotgunSprite != null)
        {
            // Calculate direction to mouse
            Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
            
            // Position shotgun
            shotgunSprite.transform.position = (Vector2)transform.position + direction * shotgunDistance;
            
            // Rotate shotgun
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shotgunSprite.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Flip shotgun sprite vertically if pointing left
            Vector3 scale = shotgunSprite.transform.localScale;
            scale.y = (direction.x < 0) ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            shotgunSprite.transform.localScale = scale;
        }
    }
    
    void Shoot()
    {
        canShoot = false;
        StartCoroutine(ShootCooldown());
        
        // Direction to mouse
        Vector2 direction = (mousePos - (Vector2)firePoint.position).normalized;
        
        // Calculate actual damage with multiplier
        int actualDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        
        // Spawn pellets with spread
        for (int i = 0; i < pelletCount; i++)
        {
            // Calculate spread
            float spread = Random.Range(-spreadAngle, spreadAngle);
            Vector2 spreadDirection = RotateVector(direction, spread * Mathf.Deg2Rad);
            
            // Create bullet
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            
            if (bullet != null)
            {
                bullet.damage = actualDamage;
                bullet.SetDirection(spreadDirection);
            }
        }
        
        // Visual feedback
        if (shotgunSprite != null)
        {
            StartCoroutine(ShotgunRecoilAnimation());
        }
    }
    
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
    
    IEnumerator ShotgunRecoilAnimation()
    {
        if (shotgunSprite != null)
        {
            Vector3 originalScale = shotgunSprite.transform.localScale;
            shotgunSprite.transform.localScale = originalScale * 0.8f;
            yield return new WaitForSeconds(0.05f);
            shotgunSprite.transform.localScale = originalScale;
        }
    }
    
    Vector2 RotateVector(Vector2 vector, float angleRad)
    {
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
    
    public void CollectCoin()
    {
        coinsCollected++;
        
        // Check for level up
        if (coinsCollected >= coinsToLevelUp)
        {
            LevelUp();
        }
        
        UpdateUI();
    }
    
    void LevelUp()
    {
        currentLevel++;
        coinsCollected = 0; // Reset coin counter
        
        // Restore health to max
        currentHealth = maxHealth;
        
        // Increase damage (stacks +10 base damage per level)
        damageMultiplier += 0.1f; // 10% more damage per level
        baseDamage += 10; // Also increase base damage
        
        Debug.Log($"Level Up! Now Level {currentLevel}. Damage: {baseDamage} x {damageMultiplier}");
        
        UpdateUI();
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Visual feedback
        StartCoroutine(DamageFlash());
        
        UpdateUI();
        
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    
    IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
    
    public void ApplyKnockback(Vector2 force)
    {
        if (rb != null)
        {
            rb.AddForce(force * knockbackResistance, ForceMode2D.Impulse);
        }
    }
    
    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealthBar(currentHealth, maxHealth);
            uiManager.UpdateExpBar(coinsCollected, coinsToLevelUp);
            uiManager.UpdateLevel(currentLevel);
        }
    }
    
    public void WinGame()
    {
        isGameOver = true;
        rb.linearVelocity = Vector2.zero;
        
        if (uiManager != null)
        {
            uiManager.ShowWinScreen();
        }
    }
    
    void GameOver()
    {
        isGameOver = true;
        rb.linearVelocity = Vector2.zero;
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver();
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw map bounds
        if (useMapBounds)
        {
            Gizmos.color = Color.cyan;
            Vector3 bottomLeft = new Vector3(minX, minY, 0);
            Vector3 bottomRight = new Vector3(maxX, minY, 0);
            Vector3 topLeft = new Vector3(minX, maxY, 0);
            Vector3 topRight = new Vector3(maxX, maxY, 0);
            
            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
    }
}