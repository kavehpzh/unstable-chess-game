using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BoardManager boardManager; // reference to board
    public float tileSize = 1f;

    private int x, y; // current grid coordinates
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
    }

    void Update()
    {
        HighlightValidMoves();
        HandleMouseInput();
    }

    // -----------------------------
    // Highlight valid moves and attack tiles
    // -----------------------------
    void HighlightValidMoves()
    {
        boardManager.HighlightTiles(piece);        // green for moves
        boardManager.HighlightAttackTiles(piece);  // orange for attack
    }

    // -----------------------------
    // Handle player clicking a tile
    // -----------------------------
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

    // -----------------------------
    // Move player or attack enemy
    // -----------------------------
    void TryMoveToTile(Vector2Int target)
    {
        // Check if clicking an enemy in range first
        Piece targetEnemy = null;
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
            // Eliminate enemy and move onto its tile
            boardManager.GetEnemiesList().Remove(targetEnemy);
            Destroy(targetEnemy.gameObject);
            x = target.x;
            y = target.y;
            piece.SetPosition(x, y, boardManager.tileSize, boardManager.transform);

            // Only now switch type
            SwitchRandomType();
            return;
        }

        // Check if target is a valid movement tile
        bool validMove = false;
        foreach (Vector2Int offset in piece.GetMovementOffsets())
        {
            if (piece.x + offset.x == target.x && piece.y + offset.y == target.y)
            {
                validMove = true;
                break;
            }
        }

        if (!validMove) return; // invalid click → do nothing

        // Move player
        x = target.x;
        y = target.y;
        piece.SetPosition(x, y, boardManager.tileSize, boardManager.transform);

        // Enemy attack check
        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int offset in enemy.GetAttackOffsets())
            {
                int ax = enemy.x + offset.x;
                int ay = enemy.y + offset.y;
                if (ax == x && ay == y)
                    Debug.Log("Game Over! You stepped into an enemy attack zone.");
            }
        }

        // Only now switch type
        SwitchRandomType();
    }


    void EliminateEnemy(Piece enemy, Vector2Int target)
    {
        boardManager.GetEnemiesList().Remove(enemy);
        Destroy(enemy.gameObject);
        MovePlayer(target);
        Debug.Log("Enemy eliminated!");
    }

    void MovePlayer(Vector2Int target)
    {
        x = target.x;
        y = target.y;
        piece.SetPosition(x, y, tileSize, boardTransform);
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

    void CheckEnemyAttacks()
    {
        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int offset in enemy.GetAttackOffsets())
            {
                int ax = enemy.x + offset.x;
                int ay = enemy.y + offset.y;
                if (ax == x && ay == y)
                {
                    Debug.Log("Game Over! You stepped into an enemy attack zone.");
                }
            }
        }
    }

    // -----------------------------
    // Randomly switch player type (unstable mechanic)
    // -----------------------------
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

        piece.SetType(newType); // Use Piece's method to update sprite
        lastType = newType;

        Debug.Log("Player piece changed to: " + newType);
    }

}
