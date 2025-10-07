using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public BoardManager boardManager; // reference to the board
    public float tileSize = 1f;

    private int x, y; // current grid coordinates
    private Transform boardTransform;

    private Piece piece;
    private PieceType lastType; // store previous type to avoid repetition

    void Start()
    {
        piece = GetComponent<Piece>();
        x = piece.x;
        y = piece.y;
        boardTransform = boardManager.transform;
        lastType = piece.type;
    }

    void Update()
    {
        int moveX = 0;
        int moveY = 0;

        // Input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) moveY = 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) moveY = -1;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) moveX = -1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) moveX = 1;

        if (moveX != 0 || moveY != 0)
        {
            TryMove(moveX, moveY);
        }
    }

    void TryMove(int moveX, int moveY)
    {
        int targetX = x + moveX;
        int targetY = y + moveY;

        // Clamp to board boundaries
        if (targetX < 0 || targetX >= boardManager.boardSize) return;
        if (targetY < 0 || targetY >= boardManager.boardSize) return;

        // Move player
        x = targetX;
        y = targetY;
        transform.position = boardTransform.position + new Vector3(x * tileSize, y * tileSize, 0);

        piece.x = x;
        piece.y = y;

        // Check enemy attack zones
        foreach (Piece enemy in boardManager.GetEnemies())
        {
            foreach (Vector2Int attack in enemy.GetAttackTiles(boardManager.boardSize))
            {
                if (attack.x == x && attack.y == y)
                {
                    Debug.Log("Game Over! You stepped into an enemy attack zone.");
                }
            }
        }

        // -----------------------------
        // UNSTABLE MECHANIC: change piece type randomly
        // -----------------------------
        SwitchRandomType();
    }

    void SwitchRandomType()
    {
        PieceType[] playerTypes = { PieceType.King, PieceType.Rook, PieceType.Pawn };

        // Filter out last type to avoid repetition
        PieceType newType;
        do
        {
            newType = playerTypes[Random.Range(0, playerTypes.Length)];
        } while (newType == lastType);

        piece.type = newType;
        lastType = newType;

        // Optional: change color to reflect type
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        switch (newType)
        {
            case PieceType.King: sr.color = Color.cyan; break;
            case PieceType.Rook: sr.color = Color.magenta; break;
            case PieceType.Pawn: sr.color = Color.yellow; break;
        }

        Debug.Log("Player piece changed to: " + newType);
    }
}
