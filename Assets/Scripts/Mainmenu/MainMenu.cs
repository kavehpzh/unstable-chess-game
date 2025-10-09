using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // This function will be called when the Exit button is clicked
    public void ExitGame()
    {
        // If we are in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If we are in a built game
            Application.Quit();
        #endif
    }
}
