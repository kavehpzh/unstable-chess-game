using UnityEngine;

public enum PieceType { Player, EnemyKing, EnemyRook, EnemyPawn } // add more later

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int x, y; // grid coordinates

    public void SetPosition(int newX, int newY, float tileSize, Transform boardTransform)
    {
        x = newX;
        y = newY;

        // Position relative to the board
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
    }
    public Vector2Int[] GetAttackTiles(int boardSize)
    {
        // Only enemies have attack tiles
        if (type == PieceType.Player) return new Vector2Int[0];

        // Example: 1-tile surrounding attack (King-style)
        System.Collections.Generic.List<Vector2Int> attacks = new System.Collections.Generic.List<Vector2Int>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // skip self
                int targetX = x + dx;
                int targetY = y + dy;

                if (targetX >= 0 && targetX < boardSize && targetY >= 0 && targetY < boardSize)
                    attacks.Add(new Vector2Int(targetX, targetY));
            }
        }

        return attacks.ToArray();
    }

}
