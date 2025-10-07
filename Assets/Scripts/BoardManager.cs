using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 5;
    public float tileSize = 1f;
    public GameObject tilePrefab;

    [Header("Pieces Settings")]
    public GameObject piecePrefab;
    public Vector2Int playerStart = new Vector2Int(0, 0);
    public Vector2Int[] enemyPositions;

    private Tile[,] tiles;
    private List<Piece> enemies = new List<Piece>();
    private Piece player;

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
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity);
                tileObj.transform.parent = transform;

                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                sr.color = ((x + y) % 2 == 0) ? Color.white : Color.gray;

                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }

        float offset = (boardSize - 1) * tileSize / 2f;
        transform.position = new Vector3(-offset, -offset, 0);
    }

    void SpawnPieces()
    {
        // --- PLAYER ---
        GameObject playerObj = Instantiate(piecePrefab);
        player = playerObj.GetComponent<Piece>();
        player.type = PieceType.Pawn; // default starting type
        player.isPlayer = true;
        player.SetPosition(playerStart.x, playerStart.y, tileSize, transform);
        playerObj.GetComponent<SpriteRenderer>().color = Color.green;

        PlayerController controller = playerObj.AddComponent<PlayerController>();
        controller.boardManager = this;
        controller.tileSize = tileSize;

        // --- ENEMIES ---
        enemies.Clear();
        foreach (Vector2Int pos in enemyPositions)
        {
            GameObject enemyObj = Instantiate(piecePrefab);
            Piece enemy = enemyObj.GetComponent<Piece>();
            enemy.type = PieceType.Pawn; // default enemy type
            enemy.isPlayer = false;
            enemy.SetPosition(pos.x, pos.y, tileSize, transform);
            enemyObj.GetComponent<SpriteRenderer>().color = Color.red;

            enemies.Add(enemy);
        }
    }

    public Piece[] GetEnemies()
    {
        return enemies.ToArray();
    }

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
                Vector3 pos = tiles[x, y].transform.position;
                Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0));
                Handles.color = Color.white;
                Handles.Label(pos + new Vector3(-0.2f, 0.2f, 0), $"{tiles[x, y].x},{tiles[x, y].y}");
            }
        }
    }
#endif
}
