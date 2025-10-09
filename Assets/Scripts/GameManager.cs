using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private LevelManager levelManager;
    
    [Header("UI Refrences")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button nextLevelButton;

    [Header("Timer Settings")]
    public float levelTime = 60f;

    private float timeRemaining;
    private bool isGameOver = false;

    [HideInInspector] public bool gameEnded = false;

    void Start()
    {
        levelManager = GetComponent<LevelManager>();
        nextLevelButton.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
        timeRemaining = levelTime;

    }

    void Update()
    {
        if (isGameOver) return;

        // countdown
        if (gameEnded == false)
        {
            timeRemaining -= Time.deltaTime;
        }
        
        if (timeRemaining < 0)
        {
            timeRemaining = 0;
            GameOver(false);
        }

        // update UI
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
    }

    public void GameOver(bool playerWon)
    {
        gameEnded = true;
        gameOverPanel.SetActive(true);

        if (playerWon)
        {
            LevelProgressManager.UnlockNextLevel(levelManager.levelNumber);
            nextLevelButton.gameObject.SetActive(true);
        }

        // Change text
        
        if (statusText != null)
            statusText.text = playerWon ? "You Win!" : "Game Over!";
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
        public void GoToNextLevel()
    {
        SceneManager.LoadScene("Level" + (levelManager.levelNumber + 1));
    }
}
