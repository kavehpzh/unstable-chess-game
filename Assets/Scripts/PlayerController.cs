using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public BoardManager boardManager;
    public float tileSize = 1f;
    private GameManager gameManager;

    private int x, y;
    private Piece piece;
    private PieceType lastType;
    private bool isMoving = false; // prevents input during animation

    void Start()
    {
        piece = GetComponent<Piece>();
        x = piece.x;
        y = piece.y;
        lastType = piece.type;

        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Update()
    {
        if (gameManager != null && gameManager.gameEnded)
            return; // stop input if game is over

        if (isMoving) return; // skip input while moving

        HighlightValidMoves();
        HandleMouseInput();
    }

    // --------------------------------------------
    // Highlight possible moves and attack zones
    // --------------------------------------------
    void HighlightValidMoves()
    {
        boardManager.ClearAllHighlights();
        boardManager.ShowMoveIndicators(piece);
        boardManager.HighlightAttackTiles(piece);
    }

    // --------------------------------------------
    // Handle player mouse input
    // --------------------------------------------
    void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int? clickedTile = GetTileFromWorldPosition(mouseWorld);

        if (clickedTile.HasValue)
            TryMoveToTile(clickedTile.Value);
    }

    Vector2Int? GetTileFromWorldPosition(Vector3 worldPos)
    {
        for (int i = 0; i < boardManager.boardSize; i++)
        {
            for (int j = 0; j < boardManager.boardSize; j++)
            {
                Tile tile = boardManager.tiles[i, j];
                if (tile == null) continue;

                Vector3 tilePos = tile.transform.position;
                float half = boardManager.tileSize / 2f;

                if (worldPos.x >= tilePos.x - half && worldPos.x <= tilePos.x + half &&
                    worldPos.y >= tilePos.y - half && worldPos.y <= tilePos.y + half)
                    return new Vector2Int(i, j);
            }
        }
        return null;
    }

    // --------------------------------------------
    // Attempt to move or attack
    // --------------------------------------------
    void TryMoveToTile(Vector2Int target)
    {
        // Clear indicators when attempting to act
        boardManager.ClearMoveIndicators();

        // --- CHECK IF TARGET IS AN ENEMY IN ATTACK RANGE ---
        Piece targetEnemy = null;

        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int offset in piece.GetAttackOffsets())
            {
                int attackX = piece.x + offset.x;
                int attackY = piece.y + offset.y;

                if (attackX == enemy.x && attackY == enemy.y &&
                    enemy.x == target.x && enemy.y == target.y)
                {
                    targetEnemy = enemy;
                    break;
                }
            }
            if (targetEnemy != null) break;
        }

        // --- ATTACK ENEMY ---
        if (targetEnemy != null)
        {
            boardManager.GetEnemiesList().Remove(targetEnemy);
            Destroy(targetEnemy.gameObject);

            StartCoroutine(MovePlayerCoroutine(target, OnPlayerMoveFinished));

            // Check if enemy was the king → player wins
            if (targetEnemy.type == PieceType.EnemyKing)
            {
                Debug.Log("You Win! Enemy King defeated.");
                if (gameManager != null)
                    gameManager.GameOver(true);
            }

            return;
        }

        // --- CHECK IF MOVE IS VALID ---
        bool validMove = false;

        foreach (Vector2Int offset in piece.GetMovementOffsets())
        {
            int tx = piece.x + offset.x;
            int ty = piece.y + offset.y;

            // Player pawn forward blocked check
            if (piece.type == PieceType.PlayerPawn && offset == new Vector2Int(0, 1))
            {
                bool blocked = false;
                foreach (Piece enemy in boardManager.GetEnemies())
                {
                    if (enemy.x == tx && enemy.y == ty)
                    {
                        blocked = true;
                        break;
                    }
                }
                if (blocked) continue;
            }

            if (tx == target.x && ty == target.y)
            {
                validMove = true;
                break;
            }
        }

        if (!validMove) return; // invalid tile → do nothing

        // --- MOVE PLAYER ---
        StartCoroutine(MovePlayerCoroutine(target, OnPlayerMoveFinished));
    }

    // --------------------------------------------
    // Coroutine to animate player movement
    // --------------------------------------------
    IEnumerator MovePlayerCoroutine(Vector2Int target, System.Action onComplete)
    {
        isMoving = true;

        Vector3 startPos = piece.transform.position;
        Vector3 endPos = boardManager.transform.position + new Vector3(target.x * tileSize, target.y * tileSize, 0);
        float duration = 0.35f; // slightly longer for better readability
        float elapsed = 0f;

        Vector3 baseScale = Vector3.one;
        float squashAmount = 0.85f;   // horizontal squash factor
        float stretchAmount = 1.15f;  // vertical stretch factor
        float hopHeight = 0.25f;      // how high it jumps (tweak for feel)

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Movement position + hop arc
            float height = Mathf.Sin(t * Mathf.PI) * hopHeight; // arc motion
            piece.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;

            // Squash & stretch based on movement phase
            if (t < 0.2f)
            {
                // take-off squash
                piece.transform.localScale = new Vector3(stretchAmount, squashAmount, 1f);
            }
            else if (t < 0.8f)
            {
                // mid-air stretch
                piece.transform.localScale = new Vector3(squashAmount, stretchAmount, 1f);
            }
            else
            {
                // landing squash
                piece.transform.localScale = new Vector3(stretchAmount, squashAmount, 1f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position and reset scale
        piece.transform.position = endPos;
        piece.transform.localScale = baseScale;

        // Update logical coordinates
        x = target.x;
        y = target.y;
        piece.x = x;
        piece.y = y;

        isMoving = false;
        onComplete?.Invoke();
    }



    // --------------------------------------------
    // Post-move logic (called after animation)
    // --------------------------------------------
    void OnPlayerMoveFinished()
    {
        // --- CHECK IF PLAYER STEPPED INTO ENEMY ATTACK ZONE ---
        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int offset in enemy.GetAttackOffsets())
            {
                int ax = enemy.x + offset.x;
                int ay = enemy.y + offset.y;
                if (ax == x && ay == y)
                {
                    Debug.Log("Game Over! You stepped into an enemy attack zone.");
                    if (gameManager != null)
                        gameManager.GameOver(false);
                    return;
                }
            }
        }

        // --- SUCCESSFUL MOVE → CHANGE PLAYER TYPE ---
        SwitchRandomType();
    }

    // --------------------------------------------
    // Randomly change player type (unstable mechanic)
    // --------------------------------------------
    void SwitchRandomType()
    {
        PieceType[] playerTypes = {
            PieceType.PlayerPawn,
            PieceType.PlayerRook,
            PieceType.PlayerKnight,
            PieceType.PlayerBishop,
            PieceType.PlayerQueen,
            PieceType.PlayerKing
        };

        PieceType newType;
        do
        {
            newType = playerTypes[Random.Range(0, playerTypes.Length)];
        } while (newType == lastType);

        piece.SetType(newType);
        lastType = newType;

        Debug.Log("Player piece changed to: " + newType);
    }
}
