using UnityEngine;
using System.Collections.Generic;

public enum PieceType { King, Rook, Pawn }

public class Piece : MonoBehaviour
{
    public PieceType type;
    public bool isPlayer;
    public int x, y;

    public void SetPosition(int newX, int newY, float tileSize, Transform boardTransform)
    {
        x = newX;
        y = newY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
    }

    // -----------------------------
    // Movement offsets relative to current position
    // -----------------------------
    public Vector2Int[] GetMovementOffsets()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        switch (type)
        {
            case PieceType.King:
                // King: 1 tile in any direction
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                            moves.Add(new Vector2Int(dx, dy));
                break;

            case PieceType.Rook:
                // Rook: horizontal and vertical lines
                for (int i = 1; i <= 4; i++) // assuming max boardSize <= 5 for now
                {
                    moves.Add(new Vector2Int(i, 0));
                    moves.Add(new Vector2Int(-i, 0));
                    moves.Add(new Vector2Int(0, i));
                    moves.Add(new Vector2Int(0, -i));
                }
                break;

            case PieceType.Pawn:
                // Pawn: 1 tile forward if player, or all directions if enemy (for simplicity)
                if (isPlayer)
                    moves.Add(new Vector2Int(0, 1)); // always forward for player
                else
                {
                    // enemy pawn attacks 1 tile forward and diagonals
                    moves.Add(new Vector2Int(0, -1));
                    moves.Add(new Vector2Int(-1, -1));
                    moves.Add(new Vector2Int(1, -1));
                }
                break;
        }

        return moves.ToArray();
    }

    // -----------------------------
    // Get attack tiles for enemies
    // -----------------------------
    public Vector2Int[] GetAttackTiles(int boardSize)
    {
        if (isPlayer) return new Vector2Int[0];

        List<Vector2Int> attacks = new List<Vector2Int>();
        foreach (Vector2Int offset in GetMovementOffsets())
        {
            int tx = x + offset.x;
            int ty = y + offset.y;
            if (tx >= 0 && tx < boardSize && ty >= 0 && ty < boardSize)
                attacks.Add(new Vector2Int(tx, ty));
        }
        return attacks.ToArray();
    }
}
