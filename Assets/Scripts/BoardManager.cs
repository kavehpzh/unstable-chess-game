using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class EnemySetup
{
    public Vector2Int position;
    public PieceType type = PieceType.EnemyPawn;
}

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 5;
    public float tileSize = 1f;
    public GameObject tilePrefab;

    [Header("Pieces Settings")]
    public GameObject piecePrefab;
    public Vector2Int playerStart = new Vector2Int(0, 0);

    [Tooltip("Define enemy positions and their piece types here")]
    public EnemySetup[] enemiesSetup;

    [Header("Move Indicator")]
    public GameObject moveIndicatorPrefab; // assign small green circle prefab in Inspector
    private List<GameObject> activeIndicators = new List<GameObject>();

    public Tile[,] tiles;
    private List<Piece> enemies = new List<Piece>();
    private Piece player;

    void Start()
    {
        GenerateBoard();
        SpawnPieces();
    }

    // -----------------------------
    // BOARD GENERATION
    // -----------------------------
    void GenerateBoard()
    {
        tiles = new Tile[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                tileObj.transform.parent = transform;

                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                Color checkerColor = ((x + y) % 2 == 0) ? Color.white : Color.darkGray;
                sr.color = checkerColor;

                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;
                tile.SetOriginalColor(checkerColor);

                tiles[x, y] = tile;
            }
        }

        float offset = (boardSize - 1) * tileSize / 2f;
        transform.position = new Vector3(-offset, -offset, 0);
    }

    // -----------------------------
    // SPAWN PIECES
    // -----------------------------
    void SpawnPieces()
    {
        // --- PLAYER ---
        GameObject playerObj = Instantiate(piecePrefab);
        player = playerObj.GetComponent<Piece>();
        player.SetType(PieceType.PlayerPawn); // starting type
        player.isPlayer = true;
        player.SetPosition(playerStart.x, playerStart.y, tileSize, transform);

        // give piece a reference back to board if needed (optional)
        player.boardManager = this;

        PlayerController controller = playerObj.AddComponent<PlayerController>();
        controller.boardManager = this;
        controller.tileSize = tileSize;

        // --- ENEMIES ---
        enemies.Clear();
        foreach (EnemySetup setup in enemiesSetup)
        {
            GameObject enemyObj = Instantiate(piecePrefab);
            Piece enemy = enemyObj.GetComponent<Piece>();
            enemy.type = setup.type;
            enemy.isPlayer = false;
            enemy.boardManager = this;
            enemy.SetPosition(setup.position.x, setup.position.y, tileSize, transform);

            if (enemy.type == PieceType.EnemyKing)
            {
                enemyObj.AddComponent<PulseColorEffect>();
            }

            enemies.Add(enemy);
        }
    }

    // -----------------------------
    // GETTERS
    // -----------------------------
    public Piece[] GetEnemies() => enemies.ToArray();
    public List<Piece> GetEnemiesList() => enemies;
    public Piece GetPlayer() => player;

    // -----------------------------
    // BOARD HELPERS
    // -----------------------------
    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < boardSize && y >= 0 && y < boardSize;
    }

    public bool IsTileOccupied(Vector2Int pos)
    {
        if (player != null && player.x == pos.x && player.y == pos.y)
            return true;

        foreach (Piece e in enemies)
            if (e.x == pos.x && e.y == pos.y)
                return true;

        return false;
    }

    public bool IsEnemyAt(Vector2Int pos)
    {
        foreach (Piece e in enemies)
            if (e.x == pos.x && e.y == pos.y)
                return true;
        return false;
    }

    // -----------------------------
    // TILE HIGHLIGHTS (kept for attack highlighting)
    // -----------------------------
    public void HighlightTiles(Piece piece)
    {
        // Reset all tiles
        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++)
                tiles[x, y].SetHighlight(false);

        foreach (Vector2Int offset in piece.GetMovementOffsets())
        {
            int tx = piece.x + offset.x;
            int ty = piece.y + offset.y;

            if (tx < 0 || tx >= boardSize || ty < 0 || ty >= boardSize)
                continue;

            // Special case: player pawn forward tile
            if (piece.type == PieceType.PlayerPawn && piece.isPlayer && offset == new Vector2Int(0, 1))
            {
                bool blocked = false;
                foreach (Piece enemy in GetEnemies())
                    if (enemy.x == tx && enemy.y == ty)
                        blocked = true;

                if (blocked) continue; // skip highlighting if blocked
            }

            tiles[tx, ty].SetHighlight(true);
        }
    }

    public void HighlightAttackTiles(Piece playerPiece)
    {
        // Clear existing attack highlights first to avoid lingering orange tiles
        for (int x = 0; x < boardSize; x++)
            for (int y = 0; y < boardSize; y++)
                tiles[x, y].SetHighlightAttack(false);

        foreach (Piece enemy in enemies)
        {
            foreach (Vector2Int offset in playerPiece.GetAttackOffsets())
            {
                int tx = playerPiece.x + offset.x;
                int ty = playerPiece.y + offset.y;

                if (tx == enemy.x && ty == enemy.y && IsInsideBoard(tx, ty))
                    tiles[tx, ty].SetHighlightAttack(true);
            }
        }
    }

    // -----------------------------
    // MOVE INDICATORS (uses GetValidMoves)
    // -----------------------------
    public void ShowMoveIndicators(Piece piece)
    {
        // if no prefab assigned, fall back to tile highlights so nothing breaks
        if (moveIndicatorPrefab == null)
        {
            HighlightTiles(piece);
            return;
        }

        ClearMoveIndicators();

        List<Vector2Int> valid = GetValidMoves(piece);

        foreach (Vector2Int target in valid)
        {
            Vector3 pos = new Vector3(target.x * tileSize, target.y * tileSize, -0.1f) + transform.position;
            GameObject indicator = Instantiate(moveIndicatorPrefab, pos, Quaternion.identity, transform);
            activeIndicators.Add(indicator);
        }
    }

    public void ClearMoveIndicators()
    {
        for (int i = activeIndicators.Count - 1; i >= 0; i--)
        {
            if (activeIndicators[i] != null)
                Destroy(activeIndicators[i]);
        }
        activeIndicators.Clear();
    }

    public void ClearAllHighlights()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                tiles[x, y].SetHighlight(false);
                tiles[x, y].SetHighlightAttack(false);
            }
        }
        ClearMoveIndicators();
    }

    // -----------------------------
    // CORE: Get valid moves with blocking (returns absolute board positions)
    // -----------------------------
    public List<Vector2Int> GetValidMoves(Piece piece)
    {
        List<Vector2Int> results = new List<Vector2Int>();
        Vector2Int origin = new Vector2Int(piece.x, piece.y);

        // helper to add single-step moves (king, knight, pawn attacks, pawn single)
        void TryAdd(Vector2Int relative)
        {
            Vector2Int pos = origin + relative;
            if (!IsInsideBoard(pos.x, pos.y)) return;

            // Pawn forward: must be empty
            if ((piece.type == PieceType.PlayerPawn || piece.type == PieceType.EnemyPawn) &&
                relative == new Vector2Int(0, piece.type.ToString().StartsWith("Enemy") ? -1 : 1))
            {
                if (!IsTileOccupied(pos))
                    results.Add(pos);
                return;
            }

            // For normal single-step moves or knight/king, add if not occupied by same-side piece
            if (!IsTileOccupied(pos))
            {
                results.Add(pos);
                return;
            }
            else
            {
                // occupied -> if occupied by an enemy, can move/attack into it
                bool isEnemyHere = IsEnemyAt(pos);
                bool isPieceEnemy = false;
                if (piece.isPlayer) isPieceEnemy = isEnemyHere;
                else isPieceEnemy = !isEnemyHere && !(player != null && player.x == pos.x && player.y == pos.y); // enemy vs player logic

                // simpler: allow moving onto a tile if it's an enemy (i.e. not same side)
                if (piece.isPlayer && IsEnemyAt(pos))
                    results.Add(pos);
                else if (!piece.isPlayer && player != null && player.x == pos.x && player.y == pos.y)
                    results.Add(pos);
            }
        }

        // Directional scanning helper (for rook/bishop/queen)
        List<Vector2Int> ScanDirection(Vector2Int dir)
        {
            List<Vector2Int> hits = new List<Vector2Int>();
            Vector2Int cur = origin;
            while (true)
            {
                cur += dir;
                if (!IsInsideBoard(cur.x, cur.y)) break;

                if (IsTileOccupied(cur))
                {
                    // if occupied by enemy relative to piece, allow capture (i.e. include tile), then stop
                    bool occupiedByEnemy = false;
                    if (piece.isPlayer)
                        occupiedByEnemy = IsEnemyAt(cur);
                    else
                        occupiedByEnemy = (player != null && player.x == cur.x && player.y == cur.y);

                    if (occupiedByEnemy)
                        hits.Add(cur);

                    break; // stop scanning past this piece
                }

                hits.Add(cur);
            }
            return hits;
        }

        // Build moves depending on type
        switch (piece.type)
        {
            case PieceType.PlayerPawn:
            case PieceType.EnemyPawn:
                {
                    // forward
                    int dir = piece.type.ToString().StartsWith("Enemy") ? -1 : 1;
                    TryAdd(new Vector2Int(0, dir));
                    // pawn attacks handled by attack offsets (we highlight attack separately)
                    break;
                }

            case PieceType.PlayerKnight:
            case PieceType.EnemyKnight:
                {
                    Vector2Int[] knightOffsets = {
                        new Vector2Int(1,2), new Vector2Int(2,1), new Vector2Int(-1,2), new Vector2Int(-2,1),
                        new Vector2Int(1,-2), new Vector2Int(2,-1), new Vector2Int(-1,-2), new Vector2Int(-2,-1)
                    };
                    foreach (var o in knightOffsets) TryAdd(o);
                    break;
                }

            case PieceType.PlayerKing:
            case PieceType.EnemyKing:
                {
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                            if (dx != 0 || dy != 0)
                                TryAdd(new Vector2Int(dx, dy));
                    break;
                }

            case PieceType.PlayerRook:
            case PieceType.EnemyRook:
                {
                    results.AddRange(ScanDirection(new Vector2Int(1, 0)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, 0)));
                    results.AddRange(ScanDirection(new Vector2Int(0, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(0, -1)));
                    break;
                }

            case PieceType.PlayerBishop:
            case PieceType.EnemyBishop:
                {
                    results.AddRange(ScanDirection(new Vector2Int(1, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(1, -1)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, -1)));
                    break;
                }

            case PieceType.PlayerQueen:
            case PieceType.EnemyQueen:
                {
                    results.AddRange(ScanDirection(new Vector2Int(1, 0)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, 0)));
                    results.AddRange(ScanDirection(new Vector2Int(0, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(0, -1)));
                    results.AddRange(ScanDirection(new Vector2Int(1, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(1, -1)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, 1)));
                    results.AddRange(ScanDirection(new Vector2Int(-1, -1)));
                    break;
                }
        }

        return results;
    }

    // -----------------------------
    // GIZMOS
    // -----------------------------
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (tiles == null) return;

        Gizmos.color = Color.yellow;

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (tiles[x, y] == null) continue;
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0) + transform.position;
                Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0));
                Handles.color = Color.white;
                Handles.Label(pos + new Vector3(-0.2f, 0.2f, 0), $"{x},{y}");
            }
        }
    }
#endif
}
