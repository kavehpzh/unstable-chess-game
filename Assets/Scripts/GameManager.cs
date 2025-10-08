using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Refrences")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text statusText;

    [Header("Timer Settings")]
    public float levelTime = 60f;

    private float timeRemaining;
    private bool isGameOver = false;

    [HideInInspector] public bool gameEnded = false;

    void Start()
    {
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

        // Change text
        
        if (statusText != null)
            statusText.text = playerWon ? "You Win!" : "Game Over!";
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
