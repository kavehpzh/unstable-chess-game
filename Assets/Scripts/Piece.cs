using UnityEngine;
using System.Collections.Generic;

public enum PieceType
{
    PlayerPawn, PlayerRook, PlayerKnight, PlayerBishop, PlayerQueen, PlayerKing,
    EnemyPawn, EnemyRook, EnemyKnight, EnemyBishop, EnemyQueen, EnemyKing
}

[RequireComponent(typeof(SpriteRenderer))]
public class Piece : MonoBehaviour
{
    [Header("Piece Data")]
    public PieceType type;
    public int x, y;
    public bool isPlayer;

    [Header("Shared Sprites")]
    public Sprite pawnSprite;
    public Sprite rookSprite;
    public Sprite knightSprite;
    public Sprite bishopSprite;
    public Sprite queenSprite;
    public Sprite kingSprite;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ApplyVisuals();
    }

    // ---------------------------------------------------
    // VISUALS
    // ---------------------------------------------------
    public void ApplyVisuals()
    {
        sr.sprite = GetSpriteForType(type);

        bool isEnemy = type.ToString().StartsWith("Enemy");
        sr.color = isEnemy ? Color.gray : Color.white;
        isPlayer = !isEnemy;
    }

    private Sprite GetSpriteForType(PieceType t)
    {
        switch (t)
        {
            case PieceType.PlayerPawn:
            case PieceType.EnemyPawn: return pawnSprite;

            case PieceType.PlayerRook:
            case PieceType.EnemyRook: return rookSprite;

            case PieceType.PlayerKnight:
            case PieceType.EnemyKnight: return knightSprite;

            case PieceType.PlayerBishop:
            case PieceType.EnemyBishop: return bishopSprite;

            case PieceType.PlayerQueen:
            case PieceType.EnemyQueen: return queenSprite;

            case PieceType.PlayerKing:
            case PieceType.EnemyKing: return kingSprite;
        }
        return null;
    }

    public void SetPosition(int newX, int newY, float tileSize, Transform boardTransform)
    {
        x = newX;
        y = newY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);
    }

    // ---------------------------------------------------
    // MOVEMENT OFFSETS
    // ---------------------------------------------------
    public List<Vector2Int> GetMovementOffsets()
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        bool enemy = type.ToString().StartsWith("Enemy");

        switch (type)
        {
            case PieceType.PlayerPawn:
            case PieceType.EnemyPawn:
                // Move forward (opposite directions for player/enemy)
                int dir = enemy ? -1 : 1;
                moves.Add(new Vector2Int(0, dir));
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
                moves.AddRange(new Vector2Int[]
                {
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
        }

        return moves;
    }

    // ---------------------------------------------------
    // ATTACK OFFSETS
    // ---------------------------------------------------
    public List<Vector2Int> GetAttackOffsets()
    {
        List<Vector2Int> attacks = new List<Vector2Int>();
        bool enemy = type.ToString().StartsWith("Enemy");

        switch (type)
        {
            case PieceType.PlayerPawn:
                attacks.Add(new Vector2Int(-1, 1));
                attacks.Add(new Vector2Int(1, 1));
                break;

            case PieceType.EnemyPawn:
                attacks.Add(new Vector2Int(-1, -1));
                attacks.Add(new Vector2Int(1, -1));
                break;

            default:
                attacks.AddRange(GetMovementOffsets());
                break;
        }

        return attacks;
    }

    public void SetType(PieceType newType)
    {
        type = newType;
        ApplyVisuals(); // instantly update sprite and color
    }
}
