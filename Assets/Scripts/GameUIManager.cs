using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // This using directive is correct
using System.Collections; 

public class GameUIManager : MonoBehaviour
{
    // A tiny value that is not zero
    private const float ALMOST_ZERO = 0.0001f;

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject creditsPanel;
    public GameObject gameplayUI;
    
    [Header("Gameplay UI Elements")]
    public Image healthBar;
    // --- FIX WAS HERE ---
    public TextMeshProUGUI healthText; 
    public Image expBar;
    // --- FIX WAS HERE ---
    public TextMeshProUGUI levelText; 
    // --- FIX WAS HERE ---
    public TextMeshProUGUI timerText; 
    // --- FIX WAS HERE ---
    public TextMeshProUGUI waveText; 
    
    private bool isPaused = false;
    private float gameTimer = 0f;
    private bool gameIsActive = false;
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ShowMainMenu();
        }
        else
        {
            StartGameplay();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameIsActive)
        {
            TogglePauseMenu();
        }
        
        if (gameIsActive && !isPaused)
        {
            gameTimer += Time.deltaTime;
            UpdateTimer();
        }
    }

    private IEnumerator FreezeTimeSafely()
    {
        // Wait for the frame to end
        yield return new WaitForEndOfFrame(); 
        
        // Use the non-zero value
        Time.timeScale = ALMOST_ZERO; 
    }
    
    void StartGameplay()
    {
        HideAllMenus();
        if (gameplayUI != null) gameplayUI.SetActive(true);
        if (waveText != null) waveText.gameObject.SetActive(false); 
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
            Time.timeScale = ALMOST_ZERO; 
            
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
        if (waveText != null) waveText.gameObject.SetActive(false);
        
        StartCoroutine(FreezeTimeSafely());
    }
    
    public void ShowWinScreen()
    {
        gameIsActive = false;
        if (winPanel != null) winPanel.SetActive(true);
        if (waveText != null) waveText.gameObject.SetActive(false); 

        StartCoroutine(FreezeTimeSafely());
    }
    
    // --- (Rest of your script is unchanged) ---
    
    public void UpdateWaveText(int currentWave, int totalWaves)
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = $"Wave: {currentWave} / {totalWaves}";
        }
    }

    public void ShowBossWaveText()
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = "BOSS WAVE";
        }
    }

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
        Time.timeScale = 1f; 
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
    
    public void ResumeGame()
    {
        TogglePauseMenu();
    }
}