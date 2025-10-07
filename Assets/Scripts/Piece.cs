using UnityEngine;
using System.Collections.Generic;

public enum PieceType { PlayerPawn, PlayerRook, PlayerKnight, PlayerBishop, PlayerQueen, PlayerKing }

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
            case PieceType.PlayerPawn:
                moves.Add(new Vector2Int(0, 1)); // forward only
                break;

            case PieceType.PlayerRook:
                for (int i = 1; i < 5; i++)
                {
                    moves.Add(new Vector2Int(i, 0));
                    moves.Add(new Vector2Int(-i, 0));
                    moves.Add(new Vector2Int(0, i));
                    moves.Add(new Vector2Int(0, -i));
                }
                break;

            case PieceType.PlayerBishop:
                for (int i = 1; i < 5; i++)
                {
                    moves.Add(new Vector2Int(i, i));
                    moves.Add(new Vector2Int(-i, i));
                    moves.Add(new Vector2Int(i, -i));
                    moves.Add(new Vector2Int(-i, -i));
                }
                break;

            case PieceType.PlayerKnight:
                moves.AddRange(new Vector2Int[] {
                    new Vector2Int(1, 2), new Vector2Int(2, 1),
                    new Vector2Int(-1, 2), new Vector2Int(-2, 1),
                    new Vector2Int(1, -2), new Vector2Int(2, -1),
                    new Vector2Int(-1, -2), new Vector2Int(-2, -1)
                });
                break;

            case PieceType.PlayerQueen:
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
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                            moves.Add(new Vector2Int(dx, dy));
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
            case PieceType.PlayerPawn:
                // Attack diagonally (forward-left, forward-right)
                attacks.Add(new Vector2Int(-1, 1));
                attacks.Add(new Vector2Int(1, 1));
                break;

            default:
                // For other pieces, attacks = moves
                attacks.AddRange(GetMovementOffsets());
                break;
        }

        return attacks;
    }
}
