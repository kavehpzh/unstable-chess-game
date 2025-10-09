using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string levelSelectScene = "LevelSelection";
    public string MainMenuScene = "MainMenu";

    public void OnPlayButton()
    {
        SceneManager.LoadScene(levelSelectScene);
    }
        public void OnBackButton()
    {
        SceneManager.LoadScene(MainMenuScene);
    }
}
