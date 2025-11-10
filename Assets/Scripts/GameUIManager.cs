using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject creditsPanel;
    public GameObject gameplayUI;
    
    [Header("Gameplay UI Elements")]
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image expBar;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    
    private bool isPaused = false;
    private float gameTimer = 0f;
    private bool gameIsActive = false;
    
    void Start()
    {
        // Check scene and initialize
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ShowMainMenu();
        }
        else
        {
            // Gameplay scene
            StartGameplay();
        }
    }
    
    void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && gameIsActive)
        {
            TogglePauseMenu();
        }
        
        // Update timer during gameplay
        if (gameIsActive && !isPaused)
        {
            gameTimer += Time.deltaTime;
            UpdateTimer();
        }
    }
    
    void StartGameplay()
    {
        HideAllMenus();
        if (gameplayUI != null) gameplayUI.SetActive(true);
        gameIsActive = true;
        gameTimer = 0f;
        Time.timeScale = 1f;
    }
    
    void HideAllMenus()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
    }
    
    public void ShowMainMenu()
    {
        HideAllMenus();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        gameIsActive = false;
    }
    
    public void ShowCredits()
    {
        HideAllMenus();
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }
    
    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }
    
    public void ShowGameOver()
    {
        gameIsActive = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void ShowWinScreen()
    {
        gameIsActive = false;
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    // UI Update Methods
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
        }
    }
    
    public void UpdateExpBar(int currentExp, int expToLevel)
    {
        if (expBar != null)
        {
            expBar.fillAmount = (float)currentExp / expToLevel;
        }
    }
    
    public void UpdateLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"LV.{level:00}";
        }
    }
    
    void UpdateTimer()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60);
            int seconds = Mathf.FloorToInt(gameTimer % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
    
    // Button Methods
    public void StartGame()
    {
        SceneManager.LoadScene("GameplayScene");
        Time.timeScale = 1f;
    }
    
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1f;
    }
    
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
    
    public void ResumeGame()
    {
        TogglePauseMenu();
    }
}