using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private const string FirstTimeKey = "HasPlayedBefore";

    // These scene names should match your actual scene names
    [Header("Scene Names")]
    public string levelSelectScene = "LevelSelection";
    public string howToPlayScene = "HowToPlay";

    void Start()
    {

    }

    // Called by the Start Button
    public void OnStartButton()
    {
        if (!PlayerPrefs.HasKey(FirstTimeKey))
        {
            PlayerPrefs.SetInt(FirstTimeKey, 1); // Mark that player has played before
            PlayerPrefs.Save();
            SceneManager.LoadScene(howToPlayScene);
        }
        else
        {
            SceneManager.LoadScene(levelSelectScene);
        }

    }

    // Called by the How To Play Button
    public void OnHowToPlayButton()
    {
        SceneManager.LoadScene(howToPlayScene);
    }

    // Optional: Exit button for completeness
    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
