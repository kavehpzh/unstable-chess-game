using UnityEngine;
using UnityEngine.UI;

public class NextPieceUIController : MonoBehaviour
{
    [SerializeField] private Image nextPieceImage;
    [SerializeField] private float fadeSpeed = 8f;

    private Sprite targetSprite;
    private Color visibleColor = Color.white;
    private Color transparentColor = new Color(1, 1, 1, 0);

    private void Awake()
    {
        if (nextPieceImage != null)
            nextPieceImage.color = transparentColor;
    }

    public void ShowNextPiece(Sprite sprite)
    {
        if (nextPieceImage == null) return;

        targetSprite = sprite;
        nextPieceImage.sprite = targetSprite;
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void HidePreview()
    {
        if (nextPieceImage == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            nextPieceImage.color = Color.Lerp(transparentColor, visibleColor, t);
            yield return null;
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            nextPieceImage.color = Color.Lerp(visibleColor, transparentColor, t);
            yield return null;
        }
    }
}
