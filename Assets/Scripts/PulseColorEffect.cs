using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PulseColorEffect : MonoBehaviour
{
    public float pulseSpeed = 5f;      // Speed of the pulse
    public Color pulseColor = Color.yellow;

    private Vector3 originalScale;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    void Update()
    {

        // Pulse color
        float colorPulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f) + 0.5f;
        sr.color = Color.Lerp(originalColor, pulseColor, colorPulse);
    }
}
