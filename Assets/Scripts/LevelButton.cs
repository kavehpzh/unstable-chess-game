using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;
    public string sceneName;
    public Button button;
    public Image lockIcon; // assign this manually in inspector!

    void Start()
    {
        GetComponentInChildren<TMP_Text>().text = "Level " + levelNumber;
        int unlockedLevel = LevelProgressManager.GetUnlockedLevel();

        bool isUnlocked = levelNumber <= unlockedLevel;
        button.interactable = isUnlocked;

        if (lockIcon != null)
            lockIcon.gameObject.SetActive(!isUnlocked);

        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        SceneManager.LoadScene(sceneName);
    }
}
