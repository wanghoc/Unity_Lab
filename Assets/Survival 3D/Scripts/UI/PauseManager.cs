using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    
    [Header("UI")]
    public GameObject pauseMenuUI; // Panel pause menu
    public GameObject settingsMenuUI; // Panel settings (optional)
    
    [Header("Settings")]
    public KeyCode pauseKey = KeyCode.Escape;
    
    private bool isPaused = false;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Ẩn menu lúc bắt đầu
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
    }
    
    void Update()
    {
        // Nhấn ESC để pause/resume
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        
        Time.timeScale = 0f; // Dừng thời gian game
        isPaused = true;
        
        // Hiện con trỏ chuột
        if (PlayerController.instance != null)
        {
            PlayerController.instance.ToggleCursor(true);
        }
        
        Debug.Log("⏸️ Game Paused");
    }
    
    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
        
        Time.timeScale = 1f; // Tiếp tục thời gian game
        isPaused = false;
        
        // Ẩn con trỏ chuột
        if (PlayerController.instance != null)
        {
            PlayerController.instance.ToggleCursor(false);
        }
        
        Debug.Log("▶️ Game Resumed");
    }
    
    public void OpenSettings()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
            
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
            
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }
    
    public void SaveAndQuit()
    {
        // Save game trước khi quit
        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.SaveGame();
        }
        
        Resume(); // Reset time scale
        SceneManager.LoadScene("Menu");
    }
    
    public void QuitToDesktop()
    {
        // Save game trước khi quit
        if (PlayerDataManager.instance != null)
        {
            PlayerDataManager.instance.SaveGame();
        }
        
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}
