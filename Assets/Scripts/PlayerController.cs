using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public BoardManager boardManager;
    public float tileSize = 1f;
    private GameManager gameManager;

    private int x, y;
    private Transform boardTransform;
    private Piece piece;
    private PieceType lastType;

    void Start()
    {
        piece = GetComponent<Piece>();
        x = piece.x;
        y = piece.y;
        boardTransform = boardManager.transform;
        lastType = piece.type;

        gameManager = FindAnyObjectByType<GameManager>();

    }

    void Update()
    {
        if (gameManager != null && gameManager.gameEnded)
            return; // stop all input if game is over

        HighlightValidMoves();
        HandleMouseInput();

    }

    // --------------------------------------------
    // Highlight possible moves and attack zones
    // --------------------------------------------
    void HighlightValidMoves()
    {
        boardManager.HighlightTiles(piece);
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
        // Check if target is an enemy in attack range
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
            MovePlayerTo(target);

            if (targetEnemy.type == PieceType.EnemyKing)
            {
                Debug.Log("You Win! Enemy King defeated.");
                if (gameManager != null)
                    gameManager.GameOver(true);
                return;
            }

            SwitchRandomType();
            return;
        }

        // --- MOVE ---
        if (!IsValidMove(target))
            return; // invalid tile → do nothing

        MovePlayerTo(target);

        // Check if player walked into enemy attack
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

        // Successful move → change type
        SwitchRandomType();
    }

    bool IsValidMove(Vector2Int target)
    {
        foreach (Vector2Int offset in piece.GetMovementOffsets())
        {
            if (piece.x + offset.x == target.x && piece.y + offset.y == target.y)
                return true;
        }
        return false;
    }

    void MovePlayerTo(Vector2Int target)
    {
        x = target.x;
        y = target.y;
        piece.SetPosition(x, y, boardManager.tileSize, boardManager.transform);
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
