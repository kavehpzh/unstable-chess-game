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
    // Highlight valid moves based on current piece type
    // -----------------------------
    void HighlightValidMoves()
    {
        boardManager.HighlightTiles(piece);       // green move tiles
        boardManager.HighlightAttackTiles(piece); // orange attack tiles
    }

    // -----------------------------
    // Detect mouse click and move
    // -----------------------------
    void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int? clickedTile = GetTileFromWorldPosition(mouseWorld);

        if (clickedTile.HasValue)
            TryMoveToTile(clickedTile.Value);
    }

    // Convert mouse position to board coordinates
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
    // Move to clicked tile if valid
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
            // Eliminate enemy
            boardManager.GetEnemiesList().Remove(targetEnemy); // we need a method to return the list reference
            Destroy(targetEnemy.gameObject);
            Debug.Log("Enemy eliminated!");

            // Move player onto the enemy's tile
            x = target.x;
            y = target.y;
            transform.position = boardManager.transform.position + new Vector3(x * boardManager.tileSize, y * boardManager.tileSize, 0);
            piece.x = x;
            piece.y = y;
        }
        else
        {
            // Check if target is a valid move
            bool valid = false;
            foreach (Vector2Int offset in piece.GetMovementOffsets())
            {
                if (piece.x + offset.x == target.x && piece.y + offset.y == target.y)
                {
                    valid = true;
                    break;
                }
            }
            if (!valid) return;

            // Move player
            x = target.x;
            y = target.y;
            transform.position = boardManager.transform.position + new Vector3(x * boardManager.tileSize, y * boardManager.tileSize, 0);
            piece.x = x;
            piece.y = y;

            // Enemy attack check
            foreach (Piece enemy in boardManager.GetEnemies())
                foreach (Vector2Int offset in enemy.GetAttackOffsets())
                {
                    int ax = enemy.x + offset.x;
                    int ay = enemy.y + offset.y;
                    if (ax == x && ay == y)
                        Debug.Log("Game Over! You stepped into an enemy attack zone.");
                }

        }

        // Unstable mechanic: change player type
        SwitchRandomType();
    }


    // -----------------------------
    // Randomly switch player type (cannot repeat last type)
    // -----------------------------
    void SwitchRandomType()
    {
        PieceType[] playerTypes = { PieceType.PlayerKing, PieceType.PlayerRook, PieceType.PlayerPawn };
        PieceType newType;

        do
        {
            newType = playerTypes[Random.Range(0, playerTypes.Length)];
        } while (newType == lastType);

        piece.type = newType;
        lastType = newType;

        // Update color to reflect type
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (newType)
        {
            case PieceType.PlayerKing: sr.color = Color.cyan; break;
            case PieceType.PlayerRook: sr.color = Color.magenta; break;
            case PieceType.PlayerPawn: sr.color = Color.yellow; break;
        }

        Debug.Log("Player piece changed to: " + newType);
    }
}
