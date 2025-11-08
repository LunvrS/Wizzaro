using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject creditsPanel;
    public GameObject gameplayUI;
    
    private bool isPaused = false;
    
    void Start()
    {
        // By default, show only main menu if we start from menu scene
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ShowMainMenu();
        }
        else
        {
            // If we're in gameplay scene
            HideAllMenus();
            if (gameplayUI != null) gameplayUI.SetActive(true);
        }
    }
    
    void Update()
    {
        // Toggle pause menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            TogglePauseMenu();
        }
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
            // Pause the game
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void StartGame()
    {
        // Load the gameplay scene
        SceneManager.LoadScene("GameplayScene");
        Time.timeScale = 1f;
    }
    
    public void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1f;
    }
    
    public void QuitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
    
    // QuitGame method removed as requested
}