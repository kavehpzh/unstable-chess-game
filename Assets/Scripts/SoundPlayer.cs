using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioClip landingSoundEffect;
    public AudioClip FailSoundEffect;

    public float volume = 1f;

    // Option 1: Play sound immediately when triggered
    public void PlayLandingSound()
    {
        AudioSource.PlayClipAtPoint(landingSoundEffect, transform.position, volume);
    }

     public void PlayFailSound()
    {
        AudioSource.PlayClipAtPoint(FailSoundEffect, transform.position, volume);
    }
}
