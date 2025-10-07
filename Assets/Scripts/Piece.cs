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
}
