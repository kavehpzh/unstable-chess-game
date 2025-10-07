using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 5;
    public float tileSize = 1f;
    public GameObject tilePrefab;

    private Tile[,] tiles;

    void Start()
    {
        GenerateBoard();
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

                // Alternate colors
                SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
                if ((x + y) % 2 == 0)
                    sr.color = Color.white;
                else
                    sr.color = Color.gray;

                // Set up Tile info
                Tile tile = tileObj.GetComponent<Tile>();
                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }

        // Center board in view
        float offset = (boardSize - 1) * tileSize / 2f;
        transform.position = new Vector3(-offset, -offset, 0);
    }

    //for debug
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // draw only in play mode (optional)
        if (tiles == null) return;

        Gizmos.color = Color.yellow;

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // Draw tile border
                Vector3 pos = tiles[x, y].transform.position;
                Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0));

#if UNITY_EDITOR
                // Draw coordinate label in Scene View
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(pos + new Vector3(-0.2f, 0.2f, 0), $"{x},{y}");
#endif
            }
        }
    }

}
