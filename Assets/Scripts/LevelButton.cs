using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public int levelNumber;         // Set this in the Inspector (1, 2, 3, ...)
    public string sceneName;        // The name of the actual level scene
    public Button button;
    
    private Image lockIcon;          // Optional: lock icon overlay

    void Start()
    {
        GetComponentInChildren<TMP_Text>().text = "Level" + levelNumber;
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
