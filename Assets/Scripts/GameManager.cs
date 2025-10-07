using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // assign in inspector

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void GameOver(bool playerWon)
    {
        gameOverPanel.SetActive(true);

        // Change text
        TMPro.TMP_Text text = gameOverPanel.GetComponentInChildren<TMPro.TMP_Text>();
        if (text != null)
            text.text = playerWon ? "You Win!" : "Game Over!";
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
