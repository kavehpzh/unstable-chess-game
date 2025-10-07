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
        boardManager.HighlightTiles(piece);
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
        // Check if clicked tile is a valid move
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
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
        piece.x = x;
        piece.y = y;

        // Check enemy attack zones
        foreach (Piece enemy in boardManager.GetEnemies())
            foreach (Vector2Int attack in enemy.GetAttackTiles(boardManager.boardSize))
                if (attack.x == x && attack.y == y)
                    Debug.Log("Game Over! You stepped into an enemy attack zone.");

        // Unstable mechanic: change player type randomly
        SwitchRandomType();
    }

    // -----------------------------
    // Randomly switch player type (cannot repeat last type)
    // -----------------------------
    void SwitchRandomType()
    {
        PieceType[] playerTypes = { PieceType.King, PieceType.Rook, PieceType.Pawn };
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
            case PieceType.King: sr.color = Color.cyan; break;
            case PieceType.Rook: sr.color = Color.magenta; break;
            case PieceType.Pawn: sr.color = Color.yellow; break;
        }

        Debug.Log("Player piece changed to: " + newType);
    }
}
