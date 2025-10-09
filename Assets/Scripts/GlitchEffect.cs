using UnityEngine;
using System.Collections;

public class GlitchEffect : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Color originalColor;

    [Header("Glitch Settings")]
    public float duration = 0.2f;
    public float intensity = 0.05f; // how strong the shake is
    public float colorShift = 0.3f; // how much color flickers

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    public void TriggerGlitch()
    {
        StopAllCoroutines();
        StartCoroutine(GlitchRoutine());
    }

    IEnumerator GlitchRoutine()
    {
        // Capture current position as baseline for the shake
        Vector3 basePos = transform.localPosition;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Random slight position shake
            Vector3 offset = new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity),
                0f
            );
            transform.localPosition = basePos + offset;

            // Random color flicker
            sprite.color = new Color(
                1f,
                1f - Random.Range(0f, colorShift),
                1f - Random.Range(0f, colorShift)
            );

            yield return null;
        }

        // Reset to normal
        transform.localPosition = basePos;
        sprite.color = originalColor;
    }
}
