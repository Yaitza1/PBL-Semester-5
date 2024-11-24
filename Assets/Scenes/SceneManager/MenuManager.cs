using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Menu Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Button References")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Settings")]
    private bool isGamePaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Initialize all panels
        InitializeMenus();
        
        // Add button listeners
        SetupButtonListeners();
    }

    private void Update()
    {
        // Handle pause menu toggle with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    #region Menu Initialization
    private void InitializeMenus()
    {
        // Hide all panels initially
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        // Pause Menu buttons
        if (resumeButton) resumeButton.onClick.AddListener(ResumeGame);
        
        // Common buttons
        if (restartButton) restartButton.onClick.AddListener(RestartLevel);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }
    #endregion

    #region Pause Menu Functions
    public void TogglePauseMenu()
    {

        isGamePaused = !isGamePaused;
        pauseMenuPanel.SetActive(isGamePaused);
        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        if (isGamePaused)
        {
            TogglePauseMenu();
        }
    }
    #endregion

    #region Game Over Functions
    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelWithTransition(SceneManager.GetActiveScene().name));
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadLevelWithTransition("MainMenu"));
    }
    #endregion

    #region Scene Loading
    private IEnumerator LoadLevelWithTransition(string sceneName)
    {
        // Fade in
        yield return StartCoroutine(SceneTransition.Instance.FadeIn());

        // Hide all panels
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Reset pause state
        isGamePaused = false;
        Time.timeScale = 1f;

        // Fade out
        yield return StartCoroutine(SceneTransition.Instance.FadeOut());
    }
    #endregion
}