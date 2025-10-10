using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public BoardManager boardManager;
    public float tileSize = 1f;
    private GameManager gameManager;

    private Image nextPieceIcon; // dynamically assigned
    private int x, y;
    private Piece piece;
    private PieceType lastType;
    private PieceType nextType;
    private SoundPlayer sp;
    private bool isMoving = false;

    void Start()
    {
        sp = FindAnyObjectByType<SoundPlayer>();
        piece = GetComponent<Piece>();
        x = piece.x;
        y = piece.y;
        lastType = piece.type;

        gameManager = FindAnyObjectByType<GameManager>();

        // dynamically find the next piece UI
        nextPieceIcon = GameObject.Find("NextPieceIcon")?.GetComponent<Image>();
        if (nextPieceIcon == null)
            Debug.LogWarning("NextPieceIcon not found in scene!");

        // pick initial next type and update preview
        nextType = GetRandomTypeDifferentFrom(lastType);
        UpdateNextPiecePreview();
    }

    void Update()
    {
        if (gameManager != null && gameManager.gameEnded) return;
        if (isMoving) return;

        HighlightValidMoves();
        HandleMouseInput();
    }

    // --- Highlight & Input Methods ---
    void HighlightValidMoves() { boardManager.ClearAllHighlights(); boardManager.ShowMoveIndicators(piece); boardManager.HighlightAttackTiles(piece); }
    void HandleMouseInput() { if (!Input.GetMouseButtonDown(0)) return; Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition); Vector2Int? clickedTile = GetTileFromWorldPosition(mouseWorld); if (clickedTile.HasValue) TryMoveToTile(clickedTile.Value); }
    Vector2Int? GetTileFromWorldPosition(Vector3 worldPos) { for (int i = 0; i < boardManager.boardSize; i++) for (int j = 0; j < boardManager.boardSize; j++) { Tile tile = boardManager.tiles[i, j]; if (tile == null) continue; Vector3 tilePos = tile.transform.position; float half = boardManager.tileSize / 2f; if (worldPos.x >= tilePos.x - half && worldPos.x <= tilePos.x + half && worldPos.y >= tilePos.y - half && worldPos.y <= tilePos.y + half) return new Vector2Int(i, j); } return null; }

    // --- Move / Attack ---
    void TryMoveToTile(Vector2Int target)
    {
        boardManager.ClearMoveIndicators();
        Piece targetEnemy = null;

        // --- CHECK FOR ATTACK ---
        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int offset in piece.GetAttackOffsets())
            {
                if (piece.x + offset.x == enemy.x && piece.y + offset.y == enemy.y &&
                    enemy.x == target.x && enemy.y == target.y)
                {
                    targetEnemy = enemy;
                    break;
                }
            }
            if (targetEnemy != null) break;
        }

        if (targetEnemy != null)
        {
            boardManager.GetEnemiesList().Remove(targetEnemy);
            Destroy(targetEnemy.gameObject);
            StartCoroutine(MovePlayerCoroutine(target, OnPlayerMoveFinished));
            if (targetEnemy.type == PieceType.EnemyKing) { gameManager?.GameOver(true); }
            return;
        }

        // --- CHECK IF MOVE IS VALID WITH BLOCKING ---
        if (!IsMoveValid(target)) return;
        Debug.Log(gameManager.gameEnded);


        StartCoroutine(MovePlayerCoroutine(target, OnPlayerMoveFinished));
    }

    bool IsMoveValid(Vector2Int target)
    {
        // Don't move onto an occupied tile (attacks handled elsewhere)
        if (boardManager.IsTileOccupied(target))
            return false;

        // Sliding pieces: rook, bishop, queen
        if (piece.type == PieceType.PlayerRook || piece.type == PieceType.PlayerBishop || piece.type == PieceType.PlayerQueen)
        {
            Vector2Int dir = new Vector2Int(
                target.x == piece.x ? 0 : (target.x - piece.x) / Mathf.Abs(target.x - piece.x),
                target.y == piece.y ? 0 : (target.y - piece.y) / Mathf.Abs(target.y - piece.y)
            );

            // Validate direction for rook/bishop
            if (dir.x == 0 && dir.y == 0) return false;
            if (piece.type == PieceType.PlayerRook && dir.x != 0 && dir.y != 0) return false;
            if (piece.type == PieceType.PlayerBishop && Mathf.Abs(target.x - piece.x) != Mathf.Abs(target.y - piece.y)) return false;

            Vector2Int pos = new Vector2Int(piece.x, piece.y);
            while (true)
            {
                pos += dir;

                // Out of bounds
                if (pos.x < 0 || pos.x >= boardManager.boardSize || pos.y < 0 || pos.y >= boardManager.boardSize)
                    return false;

                // Blocked by enemy before reaching target
                if (boardManager.IsTileOccupied(pos))
                    return false;

                // Reached target safely
                if (pos == target)
                    return true;
            }
        }
        else
        {
            // Non-sliding pieces (pawn, knight, king)
            foreach (Vector2Int offset in piece.GetMovementOffsets())
                if (piece.x + offset.x == target.x && piece.y + offset.y == target.y)
                    return !boardManager.IsTileOccupied(target);
        }

        return false;
    }




    IEnumerator MovePlayerCoroutine(Vector2Int target, System.Action onComplete)
    {
        isMoving = true;

        Vector3 startPos = piece.transform.position;
        Vector3 endPos = boardManager.transform.position + new Vector3(target.x * tileSize, target.y * tileSize, 0);
        float duration = 0.35f;
        float elapsed = 0f;
        Vector3 baseScale = Vector3.one;
        float squashAmount = 0.85f;
        float stretchAmount = 1.15f;
        float hopHeight = 0.25f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float height = Mathf.Sin(t * Mathf.PI) * hopHeight;
            piece.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

            if (t < 0.2f) piece.transform.localScale = new Vector3(stretchAmount, squashAmount, 1f);
            else if (t < 0.8f) piece.transform.localScale = new Vector3(squashAmount, stretchAmount, 1f);
            else piece.transform.localScale = new Vector3(stretchAmount, squashAmount, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = endPos;
        piece.transform.localScale = baseScale;
        x = target.x; y = target.y; piece.x = x; piece.y = y;
        isMoving = false;
        onComplete?.Invoke();
    }

    void OnPlayerMoveFinished()
    {

        Vector2Int playerPos = new Vector2Int(x, y);

        // Use the new BoardManager method
        if (boardManager.IsTileThreatened(playerPos))
        {
            gameManager?.GameOver(false);
            sp.PlayFailSound();
            return;
        }
        
        sp.PlayLandingSound();
        SwitchToNextType();
    }


    void SwitchToNextType()
    {
        GetComponent<GlitchEffect>()?.TriggerGlitch();

        piece.SetType(nextType);
        lastType = nextType;

        nextType = GetRandomTypeDifferentFrom(lastType);
        UpdateNextPiecePreview();
    }

    PieceType GetRandomTypeDifferentFrom(PieceType current)
    {
        PieceType[] playerTypes = { PieceType.PlayerPawn, PieceType.PlayerRook, PieceType.PlayerKnight, PieceType.PlayerBishop, PieceType.PlayerQueen, PieceType.PlayerKing };
        PieceType newType;
        do { newType = playerTypes[Random.Range(0, playerTypes.Length)]; } while (newType == current);
        return newType;
    }

    void UpdateNextPiecePreview()
    {
        if (nextPieceIcon == null || piece == null) return;

        Sprite s = nextType switch
        {
            PieceType.PlayerPawn => piece.pawnSprite,
            PieceType.PlayerRook => piece.rookSprite,
            PieceType.PlayerKnight => piece.knightSprite,
            PieceType.PlayerBishop => piece.bishopSprite,
            PieceType.PlayerQueen => piece.queenSprite,
            PieceType.PlayerKing => piece.kingSprite,
            _ => null
        };

        nextPieceIcon.sprite = s;
        nextPieceIcon.color = Color.white;
    }
}
