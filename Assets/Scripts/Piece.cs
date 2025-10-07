using UnityEngine;
using System.Collections.Generic;

public enum PieceType
{
    PlayerPawn, PlayerRook, PlayerKnight, PlayerBishop, PlayerQueen, PlayerKing,
    EnemyPawn, EnemyRook, EnemyKnight, EnemyBishop, EnemyQueen, EnemyKing
}

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int x, y;
    public bool isPlayer;

    public void SetPosition(int newX, int newY, float tileSize, Transform boardTransform)
    {
        x = newX;
        y = newY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
    }

    // -----------------------------
    // MOVEMENT OFFSETS
    // -----------------------------
    public List<Vector2Int> GetMovementOffsets()
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        switch (type)
        {
            // ---------------- PLAYER PIECES ----------------
            case PieceType.PlayerPawn:
                moves.Add(new Vector2Int(0, 1));
                break;

            case PieceType.PlayerRook:
            case PieceType.EnemyRook:
                for (int i = 1; i < 5; i++)
                {
                    moves.Add(new Vector2Int(i, 0));
                    moves.Add(new Vector2Int(-i, 0));
                    moves.Add(new Vector2Int(0, i));
                    moves.Add(new Vector2Int(0, -i));
                }
                break;

            case PieceType.PlayerBishop:
            case PieceType.EnemyBishop:
                for (int i = 1; i < 5; i++)
                {
                    moves.Add(new Vector2Int(i, i));
                    moves.Add(new Vector2Int(-i, i));
                    moves.Add(new Vector2Int(i, -i));
                    moves.Add(new Vector2Int(-i, -i));
                }
                break;

            case PieceType.PlayerKnight:
            case PieceType.EnemyKnight:
                moves.AddRange(new Vector2Int[] {
                    new Vector2Int(1, 2), new Vector2Int(2, 1),
                    new Vector2Int(-1, 2), new Vector2Int(-2, 1),
                    new Vector2Int(1, -2), new Vector2Int(2, -1),
                    new Vector2Int(-1, -2), new Vector2Int(-2, -1)
                });
                break;

            case PieceType.PlayerQueen:
            case PieceType.EnemyQueen:
                for (int i = 1; i < 5; i++)
                {
                    moves.Add(new Vector2Int(i, 0));
                    moves.Add(new Vector2Int(-i, 0));
                    moves.Add(new Vector2Int(0, i));
                    moves.Add(new Vector2Int(0, -i));
                    moves.Add(new Vector2Int(i, i));
                    moves.Add(new Vector2Int(-i, i));
                    moves.Add(new Vector2Int(i, -i));
                    moves.Add(new Vector2Int(-i, -i));
                }
                break;

            case PieceType.PlayerKing:
            case PieceType.EnemyKing:
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                            moves.Add(new Vector2Int(dx, dy));
                break;

            // ---------------- ENEMY-SPECIFIC MOVEMENT ----------------
            case PieceType.EnemyPawn:
                moves.Add(new Vector2Int(0, -1)); // enemies move downward
                break;
        }

        return moves;
    }

    // -----------------------------
    // ATTACK OFFSETS
    // -----------------------------
    public List<Vector2Int> GetAttackOffsets()
    {
        List<Vector2Int> attacks = new List<Vector2Int>();

        switch (type)
        {
            // Player pawn attacks upward diagonally
            case PieceType.PlayerPawn:
                attacks.Add(new Vector2Int(-1, 1));
                attacks.Add(new Vector2Int(1, 1));
                break;

            // Enemy pawn attacks downward diagonally
            case PieceType.EnemyPawn:
                attacks.Add(new Vector2Int(-1, -1));
                attacks.Add(new Vector2Int(1, -1));
                break;

            // All others: same as movement
            default:
                attacks.AddRange(GetMovementOffsets());
                break;
        }

        return attacks;
    }
}
