using UnityEngine;
using System.Collections.Generic;

public enum PieceType { King, Rook, Pawn } // single enum for both player and enemy

public class Piece : MonoBehaviour
{
    public PieceType type;
    public bool isPlayer; // true if this is the player, false if enemy
    public int x, y;      // grid coordinates

    // Set piece position on board
    public void SetPosition(int newX, int newY, float tileSize, Transform boardTransform)
    {
        x = newX;
        y = newY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
    }

    // Get attack tiles for enemies (player is ignored)
    public Vector2Int[] GetAttackTiles(int boardSize)
    {
        if (isPlayer) return new Vector2Int[0];

        List<Vector2Int> attacks = new List<Vector2Int>();

        // King-style attack: 1 tile around
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int tx = x + dx;
                int ty = y + dy;
                if (tx >= 0 && tx < boardSize && ty >= 0 && ty < boardSize)
                    attacks.Add(new Vector2Int(tx, ty));
            }
        }

        return attacks.ToArray();
    }
}
