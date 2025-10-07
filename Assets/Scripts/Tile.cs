using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color; // store original color at start
    }

    // Highlight or restore color
    public void SetHighlight(bool active)
    {
        if (active) sr.color = Color.green;  // highlight
        else sr.color = originalColor;       // restore original
    }

    // Optional: allow setting the original color externally (useful if BoardManager sets checkerboard colors after Awake)
    public void SetOriginalColor(Color color)
    {
        originalColor = color;
        sr.color = color;
    }
}
