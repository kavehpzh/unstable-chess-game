using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
    void Update()
    {
        // Replace KeyCode.Delete with the key you want
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();   // Delete all saved PlayerPrefs
            PlayerPrefs.Save();        // Ensure changes are written
            Debug.Log("All PlayerPrefs deleted!");
        }
    }
}
