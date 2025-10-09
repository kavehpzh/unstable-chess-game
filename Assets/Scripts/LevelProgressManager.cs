using UnityEngine;

public static class LevelProgressManager
{
    private const string UnlockedLevelKey = "UnlockedLevel";


    // Returns the highest unlocked level index (1-based)
    public static int GetUnlockedLevel()
    {
        // If no data yet, default to level 1 unlocked
        return PlayerPrefs.GetInt(UnlockedLevelKey, 1);
    }

    // when the player finishes a level
    public static void UnlockNextLevel(int currentLevel)
    {
        int unlocked = GetUnlockedLevel();

        // Only unlock next level if it’s higher than what’s already unlocked
        if (currentLevel >= unlocked)
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, currentLevel + 1);
            PlayerPrefs.Save();
        }
    }

    // reset progress
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);
    }
}
