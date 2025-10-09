using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGlitchEffect : MonoBehaviour
{
    private Image uiImage;
    private Color originalColor;

    [Header("Glitch Settings")]
    public Sprite[] sprites;         // Array of sprites to cycle through
    public float changeInterval = 1f; // Time between sprite changes
    public float duration = 0.2f;      // Duration of the glitch effect
    public float intensity = 5f;       // Shake amount in pixels
    public float colorShift = 0.3f;    // Color flicker amount

    private Sprite lastSprite;

    void Awake()
    {
        uiImage = GetComponent<Image>();
        originalColor = uiImage.color;
        StartCoroutine(SpriteCycleRoutine());
    }

    IEnumerator SpriteCycleRoutine()
    {
        while (true)
        {
            if (sprites.Length > 0)
            {
                Sprite newSprite;
                do
                {
                    newSprite = sprites[Random.Range(0, sprites.Length)];
                } while (sprites.Length > 1 && newSprite == lastSprite);

                uiImage.sprite = newSprite;
                lastSprite = newSprite;

                // Trigger glitch effect
                StartCoroutine(GlitchRoutine());
            }

            yield return new WaitForSeconds(changeInterval);
        }
    }

    IEnumerator GlitchRoutine()
    {
        Vector3 basePos = transform.localPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Slight UI shake (in pixels)
            Vector3 offset = new Vector3(
                Random.Range(-intensity, intensity),
                Random.Range(-intensity, intensity),
                0f
            );
            transform.localPosition = basePos + offset;

            // Color flicker
            uiImage.color = new Color(
                1f,
                1f - Random.Range(0f, colorShift),
                1f - Random.Range(0f, colorShift)
            );

            yield return null;
        }

        // Reset
        transform.localPosition = basePos;
        uiImage.color = originalColor;
    }
}
