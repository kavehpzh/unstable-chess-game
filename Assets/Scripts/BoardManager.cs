using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // for Handles.Label
#endif

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 5;          // NxN board
    public float tileSize = 1f;        // spacing
    public GameObject tilePrefab;

    [Header("Pieces")]
    public GameObject piecePrefab;
    public Vector2Int playerStart = new Vector2Int(0, 0);
    public Vector2Int[] enemyPositions;

    private Tile[,] tiles;

    void Start()
    {
        GenerateBoard();
        SpawnPieces();
    }

    void GenerateBoard()
    {
        tiles = new Tile[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // Instantiate tile
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                tileObj.transform.parent = transform;

                // Chessboard coloring
                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                sr.color = ((x + y) % 2 == 0) ? Color.white : Color.gray;

                // Tile info
                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }

        // Optional: center the board visually in scene
        float offset = (boardSize - 1) * tileSize / 2f;
        transform.position = new Vector3(-offset, -offset, 0);
    }

    void SpawnPieces()
    {
        // Spawn player
        GameObject playerObj = Instantiate(piecePrefab);
        Piece player = playerObj.GetComponent<Piece>();
        player.type = PieceType.Player;
        player.SetPosition(playerStart.x, playerStart.y, tileSize, transform); // pass board transform
        playerObj.GetComponent<SpriteRenderer>().color = Color.green;

        // Spawn enemies
        foreach (Vector2Int pos in enemyPositions)
        {
            GameObject enemyObj = Instantiate(piecePrefab);
            Piece enemy = enemyObj.GetComponent<Piece>();
            enemy.type = PieceType.EnemyPawn;
            enemy.SetPosition(pos.x, pos.y, tileSize, transform); // pass board transform
            enemyObj.GetComponent<SpriteRenderer>().color = Color.red;
        }

    }

    // for drbug
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

                Vector3 gizmoPos = tiles[x, y].transform.position;

                // Draw tile outline
                Gizmos.DrawWireCube(gizmoPos, new Vector3(tileSize, tileSize, 0));

                // Draw coordinate label above the tile
                Handles.color = Color.white;
                Handles.Label(gizmoPos + new Vector3(-0.2f, 0.2f, 0), $"{tiles[x, y].x},{tiles[x, y].y}");
            }
        }
    }
#endif
}
