using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public AudioClip sceneMusic;

    void Start()
    {
        if (MusicManager.instance != null && sceneMusic != null)
        {
            MusicManager.instance.ChangeMusic(sceneMusic);
        }
    }
}
