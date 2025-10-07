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

    // -----------------------------
    // TILE HIGHLIGHTS
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


    public void HighlightAttackTiles(Piece player)
    {
        foreach (Piece enemy in enemies)
        {
            foreach (Vector2Int offset in player.GetAttackOffsets())
            {
                int tx = player.x + offset.x;
                int ty = player.y + offset.y;

                if (tx == enemy.x && ty == enemy.y)
                    tiles[tx, ty].SetHighlightAttack(true);
            }
        }
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
